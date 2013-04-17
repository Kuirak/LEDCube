using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using SecretLabs.NETMF.Hardware;
using System.Threading;


namespace Tlc5940
{
    public class Tlc5940 
    {

        /// <summary>
        /// Illegal channel number, should be between 0 and 15 inclusive.
        /// </summary>
        public class InvalidChannelException : Exception
        {
            public InvalidChannelException(uint n) 
                : base("Channel number is out of bounds: " + n)
            {
            }
        }


        /*
         * There is an existing TLC5940 driver for the Arduino, but somehow that doesn't quite work as it should. As for use with  
         * the Netduino and the Sparkfun shield: mind that the board is not pin-compatible! (read on for details... )
         * 
         *   (I always use the pin names as mentioned on the Sparkfun shield when referring to the connections on the shield. Pin numbers 
         *    always refer to the Netduino.)
         * 
         * The TLC5940 uses two pulse trains to generate the actual PWM output signal. 
         * - A pulse on BLANK starts the counting process...
         * - ... after that, pulses on GSCLK are counted up to 4096. These are compared to the PWM output setting. While that 
         *   limit is not reached, the signal on the PWM output is kept high. It switches to low after. GSCLK on the shield is 
         *   wired to pin 3, but on the Netduino, that is not a PWM port.
         * 
         * Then, the whole game starts again. So, we need a constant flow of pulses to keep the signal properly going. There are 
         * three things to keep in mind here:
         * - if BLANK flanks before the count has been completed, the counters are reset, and start counting again. So that effectively 
         *   keeps the output high all the time.
         * - Both BLANK and GSCLK should stay low when loading new settings data. 
         * - You are actually exchanging 2 8-bit PWM channels for 16 12-bit ones, 
         * 
         * There are two ways of loading data, and I've not seen any difference in use, so this is something you need to decide based
         * on how to use the pins on the Netduino. The number of pins you need is always the same. You always have to write a full 
         * data array, so new values for all channels. 
         * - SPI interface, where we use pins 11 for sending data and 13 as serial clock.
         * - any pair of pins, and just write pulses ourselves. 
         * When data has been loaded, a pulse on XLAT makes the data effective on the outputs. The SPI pins can be connected to other devices,
         * the TLC will just keep pushing the bits through, until we push out all old bits with a fresh write. We do not need the slave select 
         * pin, so we just set that to any pin. 
         * 
         * To summarize:
         * - With SPI:               pins 11, 13, output pin for XLAT, PWM pins for BLANK, GSCLK
         * - Write to pins directly: output pins for XLAT, SIN, SCLK, PWM pins for BLANK, GSCLK
         * We need just the pin numbers for the categories, internally, the detailed pin assignments aren't important.
         * 
         * 
         * Now, that's a lot of text. The actual code is not so hard!
         * 
         * 
         * This is how I connect things:
         * 
         * 
         * 
         * 
         * 
         * \\todo: allow explicit configuration of the feeding PWM trains timing config
         * 
         * Not taken care of in this version:
         * - chaining TLC's (I only have one now)
         * - Using the VPRG to set the dot correction (the Sparkfun shield hides this pin, although there is a hole to connect to it, I think)
         * 
         */



        #region C'tors

        /// <summary>
        /// Create a tlc device, and configure it to communicate on the SPI interface. Set the SPI settings to neutral:
        /// new SPI.Configuration(Pins.GPIO_PIN_D8, false, 0, 0, ...  , where the pin number can be the latch pin 
        /// 
        /// </summary>
        /// <param name="config">The SPI configuration. </param>
        /// <param name="PWMchannel1">Channel for the GSCLK pin</param>
        /// <param name="PWMChannel2">Channel for the BLANK pin</param>
        /// <param name="LATCHpin">Required output channel</param>
        public Tlc5940(SPI.Configuration config, PWM gsclk, PWM blank, OutputPort LATCHport)
        {
            useSPIInterface = true;

            SPIDevice = config;
            SPIBus = new SPI(SPIDevice);

            GSCLKPin = gsclk;
            BLANKPin = blank;
            XLATpin = LATCHport;

            // Clear the channels, and disable the output
            GSCLKPin.SetDutyCycle(0);
            BLANKPin.SetDutyCycle(0);
            XLATpin.Write(false);

            GSCLKPin.SetPulse(gsclk_period, 1);
            BLANKPin.SetPulse((gsclk_period * 4096), 1);
        
        }

        public Tlc5940(PWM gsclk, PWM blank, OutputPort xlat, OutputPort sin, OutputPort sclk)
        {
            GSCLKPin = gsclk;
            BLANKPin = blank;
            XLATpin = xlat;
            SINPin = sin;
            SCLKPin = sclk;

            // Clear the channels, and disable the output
            GSCLKPin.SetDutyCycle(0);
            BLANKPin.SetDutyCycle(0);
            XLATpin.Write(false);
            SINPin.Write(false);
            SCLKPin.Write(false);

            GSCLKPin.SetPulse(gsclk_period, 1);
            BLANKPin.SetPulse((gsclk_period * 4096), 1);
        
        }


        #endregion

        #region Properties

        private bool useSPIInterface = false;
        
        // spi:
        SPI.Configuration SPIDevice;
        SPI SPIBus;

        // direct writing
        OutputPort SINPin;
        OutputPort SCLKPin;

        // for both configs:
        PWM BLANKPin;
        PWM GSCLKPin;
        OutputPort XLATpin;

        /// <summary>
        /// Packed array of the channel data.
        /// ch 15 = byte [0] + first half of  [1]
        ///    14 = second half of [1] and    [2]
        /// etc. down to 0
        /// </summary>
        byte[] DataBuffer = new byte[24]; // = 16 channels * 12 bits

        #endregion


        #region Data access

        /// <summary>
        /// Write data. Use the lowest 12 bits of value, rest is ignored.
        /// </summary>
        /// <param name="Channel"></param>
        /// <param name="Value"></param>
        public void SetValue(uint Channel, uint Value)
        {
            if ((Channel >= 0) && (Channel <= 15))
            {
                // We need to feed in the MSB from the highest channel first. If we map the data byte by byte, it will work
                // like this: 
                uint ChannelIndex = 15 - Channel;

                // even channels = byte + halfbyte, odd channels = halfbyte + byte
                uint set = ChannelIndex / 2;
                uint offset = ChannelIndex % 2;  // even: offset 0, odd: offset 1
                uint BufferPointer = set * 3;

                // Mask out bits to write
                uint WriteVal = Value & 0xFFF;

                if (offset == 0)
                {
                    DataBuffer[3 * set] = (byte)(WriteVal >> 4);
                    DataBuffer[3 * set + 1] = (byte)(((WriteVal & 0xF) << 4) + (DataBuffer[3 * set + 1] & 0xF));
                }
                else
                {
                    DataBuffer[3 * set + 1] = (byte)((WriteVal >> 8) + (DataBuffer[3 * set + 1] & 0xF0));
                    DataBuffer[3 * set + 2] = (byte)(WriteVal & 0xFF);
                }
            }
            else
            {
                throw new InvalidChannelException(Channel);
            }

        }

        /// <summary>
        /// Return a copy of the data array, for testing purposes.
        /// </summary>
        /// <returns></returns>
        public byte[] ShowBuffer()
        {
            return (byte[]) DataBuffer.Clone();
        }

        uint gsclk_period = 2;


        /// <summary>
        /// Send the current data buffer to the device.
        /// </summary>
        public void UpdateChannel()
        {


            if (useSPIInterface)
            {
                SPIBus.Write(DataBuffer);
            }
            else
            {
                // write each of the bytes, and pulse the clock pin
                for (int i = 0; i < DataBuffer.Length; i++)
                { 
                    byte j = 0x80;
                    for (int k = 0; k < 8; k++)
                    {
                        SINPin.Write((DataBuffer[i] & j) != 0);
                        SCLKPin.Write(true);
                        SCLKPin.Write(false);
                        j = (byte)(j >> 1);
                    }
                }
            }

            // pull down the PWM drivers
            //BLANKPin.SetPulse(10, 10); <- 

            // push the data from the input buffer on the TLC to the internal registers
            XLATpin.Write(true);
            XLATpin.Write(false);

            // push the PWM drivers back up

            // THis is the arduino formula:
            //BLANKPin.SetPulse((gsclk_period + 1) * (4096 / 2), 1);

            // This is my formula. No offense, but it behaves good for me! An offset of even 1 on the total period makes the 
            // output flicker!!! The combination 3 and 8192 definitely gives bad flickering results.
            GSCLKPin.SetPulse(gsclk_period, 1);
            BLANKPin.SetPulse((gsclk_period * 4096), 1);
        }

        #endregion

    }
}

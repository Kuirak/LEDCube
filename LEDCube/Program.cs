using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;


namespace LEDCube
{
    public class Program
    {
        

        public static void Main()
        {
            // now use the driver 

            var device1 = new SPI.Configuration(
                Pins.GPIO_PIN_D8, // SS-pin                          = slave select, not used
                false,             // SS-pin active state
                0,                 // The setup time for the SS port
                0,                 // The hold time for the SS port     
                true,              // The idle state of the clock
                true,             // The sampling clock edge
                2000,              // The SPI clock rate in KHz         ==> enough to start with, let's see how high we can take this later on
                 SPI_Devices.SPI1   // The used SPI bus (refers to a MOSI MISO and SCLK pinset)   => default pins, also the ones used on the pwm shield
                // specifically: sin = 11, (netduino send, tlc in) and sclck = 13
            );
            // better to pass in the pins, and let the ports and pwms be managed insode the device

            var latch = new OutputPort(Pins.GPIO_PIN_D9, false);
            var signal = new OutputPort(Pins.GPIO_PIN_D2, false);
            var blank = new PWM(Pins.GPIO_PIN_D10);
            var gsclk = new PWM(Pins.GPIO_PIN_D5);
            //OutputPort sin = new OutputPort(Pins.GPIO_PIN_D11, false);
            //OutputPort sclk = new OutputPort(Pins.GPIO_PIN_D13, false);


            uint value = 0;
            int step = 16;

            var pwmDevice = new Tlc5940.Tlc5940(device1, gsclk, blank, latch);
            //Tlc5940.Tlc5940 PwmDevice = new Tlc5940.Tlc5940(gsclk, blank, latch, sin, sclk);

            var foo = false;
            while (true)
            {
                // Hook up to an LED on channel 2, just to confirm that things are alive.
                signal.Write(foo);
                foo = !foo;

                //var val = (uint)(4095 * value / 100.0);

                for (uint i = 0; i < 16; i++)
                {
                    pwmDevice.SetValue(i, value);
                }
                pwmDevice.UpdateChannel();



                value =(uint) (step+value);

                if (value >= 4095|| value <=0)
                {
                    step = -step;
                }
                Thread.Sleep(10);

            }
        }
    }
}

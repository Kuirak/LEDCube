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

        static uint row = 0;
        static uint rowLength = 18;
        public static void Main()
        {
            // now use the driver 

            // now use the driver 

            var Device1 = new SPI.Configuration(
                Pins.GPIO_PIN_D8, // SS-pin                          = slave select, not used
                false, // SS-pin active state
                0, // The setup time for the SS port
                0, // The hold time for the SS port     
                false, // The idle state of the clock
                true, // The sampling clock edge
                30000,
                // The SPI clock rate in KHz         ==> enough to start with, let's see how high we can take this later on
                SPI_Devices.SPI1
                // The used SPI bus (refers to a MOSI MISO and SCLK pinset)   => default pins, also the ones used on the pwm shield
                // specifically: sin = 11, (netduino send, tlc in) and sclck = 13
                );
            // better to pass in the pins, and let the ports and pwms be managed insode the device

            var latch = new OutputPort(Pins.GPIO_PIN_D9, false);
            var blank = new PWM(Pins.GPIO_PIN_D10);
            var gsclk = new PWM(Pins.GPIO_PIN_D6);
            //OutputPort sin = new OutputPort(Pins.GPIO_PIN_D11, false);
            //OutputPort sclk = new OutputPort(Pins.GPIO_PIN_D13, false);
            
            var led = new OutputPort(Pins.ONBOARD_LED, false);
            var button = new InterruptPort(Pins.ONBOARD_SW1, true, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);

            button.OnInterrupt +=ButtonOnOnInterrupt;
             

            int percr = 100;
            int percg = 49;
            int percb = 25;
            int stepr = 1;
            int stepg = 1;
            int stepb = 1;

            uint start = 4; //The first 4 channels are empty
            uint end = 107; //
            var tlc5940 = new Tlc5940.Tlc5940(Device1, gsclk, blank, latch);
            //Tlc5940.Tlc5940 PwmDevice = new Tlc5940.Tlc5940(gsclk, blank, latch, sin, sclk);

            //UDN
            var layers = new[]
                         {
                             new OutputPort(Pins.GPIO_PIN_D0, false),
                             new OutputPort(Pins.GPIO_PIN_D1, false),
                             new OutputPort(Pins.GPIO_PIN_D2, false),
                             new OutputPort(Pins.GPIO_PIN_D3, false),
                             new OutputPort(Pins.GPIO_PIN_D4, false),
                             new OutputPort(Pins.GPIO_PIN_D5, false)
                         };
           

            var ledtoggle = true;
            
            for (uint i = 0; i < 111; i ++)
            {
                tlc5940.SetValue(i, 0);
            }
            tlc5940.UpdateChannel();
            uint index = row;
            uint timer = 0;
            while (true)
            {
                // Hook up to an LED on channel 2, just to confirm that things are alive.
                ledtoggle = !ledtoggle;
                led.Write(ledtoggle);
                
                timer++;
                if (timer%10 == 0)
                {
                    index += rowLength;
                    
                }
               
                if (index == end)
                {
                    index--;
                }
                if (index > end )
                {
                    index = row;
                }

                var valr = (uint) (4095*percr/100.0);
                //var valg = (uint)(4095 * percg / 100.0);
                //var valb = (uint)(4095 * percb / 100.0);

                for (uint i = 0; i < end; i++)
                {
                    tlc5940.SetValue(i, 0);
                }
                tlc5940.UpdateChannel();
                foreach (var layer in layers)
                {
                    layer.Write(true);
                    tlc5940.SetValue(index, valr);
                    tlc5940.UpdateChannel();
                    
                    layer.Write(false);
                }

                
                


                //for (uint i = 1; i < 16; i ++)
                //{
                //    PwmDevice.SetValue(i, valr);
                //}
                //for (uint i = 2; i < 16; i+=4)
                //{
                //    PwmDevice.SetValue(i, valb);
                //}


                //if (percr == 25 || percr == 0)
                //{
                //    stepr = -stepr;
                //}
                //percr += stepr;

                //}  if (percb == 50||percb==0)
                    //{
                    //    stepb = -stepb;

                    //}
                    
                    //break;
                
                //Thread.Sleep(100000);
            }
        }

        private static DateTime lastButtonTime = DateTime.Now;
        private static void ButtonOnOnInterrupt(uint port, uint data, DateTime time)
        {
            if (data == 1)
            {
                if ((time - lastButtonTime).Milliseconds < 100)
                    return;

                row++;
                if (row >= rowLength)
                {
                    row = 0;
                    
                }
                Debug.Print("Current row: "+row);
                lastButtonTime = time;
            }
        }
    }
}
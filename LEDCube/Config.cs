using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;


namespace LEDCube
{
    public static class Config
    {
        public static OutputPort Latch ;
        public static SPI.Configuration Device;
        public static PWM Blank;
        public static PWM Gsclk;
        public static OutputPort[] Layers;

        static Config()
        {
            Latch = new OutputPort(Pins.GPIO_PIN_D9, false);
            Device = new SPI.Configuration(
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

            Blank = new PWM(Pins.GPIO_PIN_D10);
            Gsclk = new PWM(Pins.GPIO_PIN_D6);
            
            Layers=new[]
                         {
                             new OutputPort(Pins.GPIO_PIN_D0, false),
                             new OutputPort(Pins.GPIO_PIN_D1, false),
                             new OutputPort(Pins.GPIO_PIN_D2, false),
                             new OutputPort(Pins.GPIO_PIN_D3, false),
                             new OutputPort(Pins.GPIO_PIN_D4, false),
                             new OutputPort(Pins.GPIO_PIN_D5, false)
                         };
        }




    }
}

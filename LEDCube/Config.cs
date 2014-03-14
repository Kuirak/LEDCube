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
        public static OutputPort[] LayerPorts;
        public static uint TlcChannelCount;
        public static uint SideLength;

        static Config()
        {
            Latch = new OutputPort(Pins.GPIO_PIN_D9, false);
            Device = new SPI.Configuration(
                ChipSelect_Port: Pins.GPIO_NONE, // SS-pin                          = slave select, not used
                ChipSelect_ActiveState: false, // SS-pin active state
                ChipSelect_SetupTime: 0, // The setup time for the SS port
                ChipSelect_HoldTime: 0, // The hold time for the SS port     
                Clock_IdleState: true, // The idle state of the clock
                Clock_Edge: true, // The sampling clock edge
                Clock_RateKHz: 30000,
                // The SPI clock rate in KHz         ==> enough to start with, let's see how high we can take this later on
                SPI_mod: SPI.SPI_module.SPI1
                // The used SPI bus (refers to a MOSI MISO and SCLK pinset)   => default pins, also the ones used on the pwm shield
                // specifically: sin = 11, (netduino send, tlc in) and sclck = 13
                );
            // better to pass in the pins, and let the ports and pwms be managed insode the device
           
            Blank = new PWM(Pins.GPIO_PIN_D10);
            Gsclk = new PWM(Pins.GPIO_PIN_D6);
            
            LayerPorts=new[]
                         {
                             new OutputPort(Pins.GPIO_PIN_D5, false),
                             new OutputPort(Pins.GPIO_PIN_D4, false),
                             new OutputPort(Pins.GPIO_PIN_D3, false),
                             new OutputPort(Pins.GPIO_PIN_D2, false),
                             new OutputPort(Pins.GPIO_PIN_D7, false),
                             new OutputPort(Pins.GPIO_PIN_D8, false)
                         };
            SideLength = 6;
            TlcChannelCount = 112; 
           
        }




    }
}

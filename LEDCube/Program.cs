using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using SmudgeIT.Tlc5940;

namespace LEDCube
{
    public class Program
    {
        private static Tlc5940 chip;

        public static void Main()
        {
            chip = new Tlc5940(
                                  Pins.GPIO_PIN_D13,
                                  Pins.GPIO_PIN_D11,
                                  Pins.GPIO_PIN_D10,
                                  Pins.GPIO_PIN_D9,
                                  Pins.GPIO_PIN_D8,
                                  Pins.GPIO_PIN_D12
                                  );
            chip.DotCorrectionValues = new int[] { 3, 1,5, 3, 1, 5, 3, 1, 5, 3, 1, 5, 3, 1, 5, 1 };
            chip.Init();
            chip.SetAll(0);
            chip.Update();
            var active = 1;
            while (true)
            {
                //chip.Update();
                chip.SetAll(0);
                //chip.Set(1+2,100);
                //chip.Set(4+2,100);
                //chip.Set(7+2,100);
                //chip.Set(10+2,100);
                var counter = 100;
                //while (counter >0)
                //{
                //    counter--;
                    chip.Set(active, 100);
                    chip.Update();
                //}
                Thread.Sleep(1000);
                active+=3;
                if (active == 10+3)
                { active = 2; }
                else if (active == 11+3)
                { active = 3; }
                else if (active == 12+3)
                { active = 1; }
            }
        }
    }
}

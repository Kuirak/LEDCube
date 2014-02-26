﻿using System;
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
            //OutputPort sin = new OutputPort(Pins.GPIO_PIN_D11, false);
            //OutputPort sclk = new OutputPort(Pins.GPIO_PIN_D13, false);
            
            var led = new OutputPort(Pins.ONBOARD_LED, false);
            var button = new InterruptPort(Pins.ONBOARD_SW1, true, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);

            button.OnInterrupt +=ButtonOnOnInterrupt;
             

           

            
            var tlc5940 = new Tlc5940(112);
            //Tlc5940.Tlc5940 PwmDevice = new Tlc5940.Tlc5940(gsclk, blank, latch, sin, sclk);

           
           
            
           

           
            ThreadStart ledBlink = () =>
                                   {
                                       var ledtoggle = true;
                                       while (true)
                                       {
                                           // Hook up to an Onboard LED , just to confirm that things are alive.
                                           ledtoggle = !ledtoggle;
                                           led.Write(ledtoggle);
                                           Thread.Sleep(200);
                                       }
                                   };
            new Thread(ledBlink).Start();


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
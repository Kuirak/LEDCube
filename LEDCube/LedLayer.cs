using System;
using System.Collections;
using System.Threading;

namespace LEDCube
{
    public class LedLayer
    {
        public readonly int index;
        public Led[] Leds;
       

        public LedLayer(int index)
        {
            this.index = index;
            var leds = new ArrayList();
            uint usedChannelCount = Config.SideLength*Config.SideLength*3; // * 3 cause of rgb
            var LedIndex = 0;
            for (uint i = 0; i < usedChannelCount; i += 3)
            {
                leds.Add(new Led(i,this,LedIndex));
                LedIndex++;
            }
            Leds = (Led[]) leds.ToArray(typeof (Led));
        }


        public void WriteBufferFromLeds( ref byte[] buffer)
        {
            foreach (var led in Leds)
            {
                SetValues(led,ref buffer);
                Thread.Sleep(1);
            }
        }

        public void SetAllLedsColor(float r, float g, float b)
        {
            foreach (var led in Leds)
            {
                led.Set(r,g,b);
            }
        }


        private static void SetValues(Led led, ref byte[] buffer)
        {
            TransferTo12BitBuffer(led.RedIndex, led.Red, buffer);
            TransferTo12BitBuffer(led.GreenIndex, led.Green, buffer );
            TransferTo12BitBuffer(led.BlueIndex, led.Blue, buffer);
        }


        private static void TransferTo12BitBuffer(uint index, uint value, byte[] buffer)
        {
            Tlc5940.WriteTo12BitBuffer(index, value, buffer, Config.TlcChannelCount);
        }

        /// <summary>
        /// Get Led by zerobased X and Y coord
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Led GetLed(int x, int y)
        {
            var idx = x*Config.SideLength + y;
            if (Leds.Length > idx)
            {
                return Leds[idx];
            }
            return null;
        }
    }
}
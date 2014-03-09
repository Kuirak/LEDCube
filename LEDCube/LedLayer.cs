using System.Collections;

namespace LEDCube
{
    public class LedLayer
    {
        public Led[] Leds;

        public LedLayer()
        {
            var leds = new ArrayList();

            uint usedChannelCount = Config.SideLength*Config.SideLength*3; // * 3 cause of rgb
            for (uint i = 0; i < usedChannelCount; i += 3)
            {
                leds.Add(new Led(i));
            }
            Leds = (Led[]) leds.ToArray(typeof (Led));
        }


        public void WriteBufferFromLeds(byte[] buffer)
        {
            foreach (var led in Leds)
            {
                SetValues(led,buffer);
            }
        }

        public void SetAllLedsColor(float r, float g, float b)
        {
            foreach (var led in Leds)
            {
                led.Set(r,g,b);
            }
        }


        private static void SetValues(Led led, byte[] buffer)
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
        public Led GetLed(uint x, uint y)
        {
            var index = x * Config.SideLength + y;
            return Leds[index];
        }
    }
}
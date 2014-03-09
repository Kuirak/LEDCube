using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace LEDCube
{
    public class Layer
    {
        private readonly uint sideLength;
        public readonly OutputPort Port;
       
        public byte[] LayerBuffer12Bit;
        public Led[] Leds;

        public Layer(uint sideLength, OutputPort port)
        {
            this.sideLength = sideLength;
            this.Port = port;
            
            var channelCount = sideLength * sideLength * 3;
            var leds = new ArrayList();
            LayerBuffer12Bit = Tlc5940.CreateBuffer(Config.TlcChannelCount);
            for (uint i = 0; i < channelCount; i += 3)
            {
                leds.Add(new Led(i));
            }
            Leds = (Led[])leds.ToArray(typeof(Led));
        }

        public void On()
        {
            Port.Write(true);
        }

        public void Off()
        {
            Port.Write(false);
        }

        public void SetValues(uint x, uint y)
        {
            var led = GetLed(x, y);
            TransferTo12BitBuffer(led.redIndex,led.Red);
            TransferTo12BitBuffer(led.greenIndex,led.Green);
            TransferTo12BitBuffer(led.blueIndex,led.Blue);
        }

        public void TransferTo12BitBuffer(uint index, uint value)
        {
            Tlc5940.WriteTo12BitBuffer(index,value,LayerBuffer12Bit,Config.TlcChannelCount);
        }

        /// <summary>
        /// Get Led by zerobased X and Y coord
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Led GetLed(uint x, uint y)
        {
            var index = x * sideLength + y;
            return Leds[index];
        }
    }
}

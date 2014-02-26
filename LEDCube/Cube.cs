using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Math = System.Math;

namespace LEDCube
{
    public class Cube
    {
        private readonly Tlc5940 tlc5940;
        private readonly Layer[] layers;

        private uint currentLayer;
        public uint CurrentLayer
        {
            get { return currentLayer; }
            set
            {
                layers[currentLayer].Off();
                currentLayer = value.Repeat(0,(uint)layers.Length-1);
            }
        }

        public Cube(uint sideLength,Tlc5940 tlc5940)
        {
            this.tlc5940 = tlc5940;
            
            layers= new Layer[sideLength];
            for (int i = 0; i < sideLength; i++)
            {
                layers[i] = new Layer(sideLength,Config.Layers[i], tlc5940);

            }
        }

        public void Start()
        {
            uint currentLed = 0;
            var rand = new Random();
            while (true)
            {
                tlc5940.Reset();
                
                foreach (var layer in layers)
                {
                    
                    uint x = (uint) rand.Next(5);
                    uint y = (uint) rand.Next(5);
                    var r = ((uint) rand.Next(100)).Clamp(25,100);
                    var g = ((uint)rand.Next(100)).Clamp(25, 100);
                    var b = ((uint)rand.Next(100)).Clamp(25, 100);
                    layer.GetLed(x, y).Set(r,g,b);
                    layer.Push();
                    layer.On();
                    layer.Off();
                }
                Thread.Sleep(15);
            }
        }
        /// <summary>
        /// set color for zerobased coord
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void SetColorFor(uint x, uint y, uint z, uint r, uint g, uint b)
        {
            var layer = layers[z];
            var led = layer.GetLed(x, y);
            led.SetAndApplyToBuffer(r,g,b);
        }
    }

    public class Layer
    {
        private readonly uint sideLength;
        private readonly OutputPort port;
        private readonly Tlc5940 tlc5940;
        public  Led[] Leds;
        public Layer(uint sideLength, OutputPort port, Tlc5940 tlc5940)
        {
            this.sideLength = sideLength;
            this.port = port;
            this.tlc5940 = tlc5940;
            var channelCount = sideLength * sideLength * 3;
            var leds = new ArrayList();
            
            for (uint i = 0; i < channelCount; i += 3)
            {
                leds.Add(new Led(i, tlc5940));
            }
            Leds = (Led[]) leds.ToArray(typeof (Led));
        }

        public void On()
        {
            port.Write(true);
        }

        public void Push()
        {
            foreach (var led in Leds)
            {
                led.SetValues();
                led.Off();
            }
            tlc5940.UpdateChannel();
            tlc5940.Reset();
        }

        public void Off()
        {
            port.Write(false);
        }

        public void AllLedsOn()
        {
            foreach (var led in Leds)
            {
                led.On();
                led.SetValues();
            }
            Push();
        }
 
        public void AllLedsOff()
        {
            foreach (var led in Leds)
            {
                led.Off();
                led.SetValues();
            }
            Push();
        }

        public void SetAllLedsColor(uint red,uint green,uint blue)
        {
            foreach (var led in Leds)
            {
                led.Set(red,green,blue);
                led.SetValues();
            }
            Push();
        }
        /// <summary>
        /// Get Led by zerobased X and Y coord
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Led GetLed(uint x,uint y)
        {
            var index = x*sideLength + y;
            return Leds[index];
        }
    }

  

}

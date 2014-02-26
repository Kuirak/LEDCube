using System;
using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Math = System.Math;

namespace LEDCube
{
    public class Cube
    {
        private readonly Tlc5940 tlc5940;
        private Layer[] Layers;
    
        public Cube(uint sideLength,Tlc5940 tlc5940)
        {
            this.tlc5940 = tlc5940;
            
            Layers= new Layer[sideLength];
            for (int i = 0; i < sideLength; i++)
            {
                Layers[i] = new Layer(sideLength, tlc5940);
            }
           
        }
    }

    public class Layer
    {
        private readonly Tlc5940 tlc5940;
        public  Led[] Leds;
        public OutputPort Port;
        public Layer(uint sideLength, Tlc5940 tlc5940)
        {
            this.tlc5940 = tlc5940;
            var channelCount = sideLength * sideLength * 3;
            var leds = new ArrayList();
            
            for (uint i = 0; i < channelCount; i += 3)
            {
                leds.Add(new Led(i, tlc5940));
            }
            Leds = (Led[]) leds.ToArray(typeof (Led));
        }
    }

}

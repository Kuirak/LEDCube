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
            while (true)
            {
                //if(frame.ready)
                // setcurrentlayerbuffers

                for (int index = 0; index < layers.Length; index++)
                {
                    //Set current layer buffer and 
                    tlc5940.Reset(); // set all zero
                    tlc5940.AllOne(); //set all one and write to tlc

                    layers[index].Port.Write(true); //layer on
                    Thread.Sleep(1);
                    layers[index].Port.Write(false); //layer off
                }
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
            led.Set(r,g,b);
        }
    }

}

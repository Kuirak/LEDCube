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
        private readonly LedLayer[] LedLayers;
  

        public Cube(Tlc5940 tlc5940)
        {
            this.tlc5940 = tlc5940;
            layers= new Layer[Config.SideLength];
            LedLayers = new LedLayer[Config.SideLength];
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new Layer(Config.LayerPorts[i]);
                LedLayers[i] = new LedLayer();
            }
            LedLayers[0].SetAllLedsColor(0,0,0);
            LedLayers[1].SetAllLedsColor(1,0.5f,0);
            LedLayers[2].SetAllLedsColor(0,0,0);
            LedLayers[3].SetAllLedsColor(0,0.5f,1f);
            LedLayers[4].SetAllLedsColor(0,0,0);
            LedLayers[5].SetAllLedsColor(0.5f,0,0.7f);
            for (int index = 0; index < LedLayers.Length; index++)
            {
                 LedLayers[index].WriteBufferFromLeds(layers[index].LayerBuffer12Bit);
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
                    var layer = layers[index];
                    tlc5940.PushBuffer(layer.LayerBuffer12Bit);
                    
                    var previousIndex = index - 1;
                    if (previousIndex < 0)
                    {
                        previousIndex = layers.Length - 1;
                    }
                    
                    //layer-1 off
                    layers[previousIndex].Off();
                    //confirm
                    tlc5940.Confirm();
                    
                    //layer on
                    layer.On();
                    
                    Thread.Sleep(1);
                }
            }
        }
    }

}

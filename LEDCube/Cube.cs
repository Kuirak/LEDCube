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
        
        private readonly Animation animation;

        public Cube(Tlc5940 tlc5940,Animation anim)
        {
            this.tlc5940 = tlc5940;
            layers = Helper.CreateLayers();
            animation = anim;
            //LedLayers[0].SetAllLedsColor(0,0,0);
            //LedLayers[1].SetAllLedsColor(1,0.5f,0);
            //LedLayers[2].SetAllLedsColor(0,0,0);
            //LedLayers[3].SetAllLedsColor(0,0.5f,1f);
            //LedLayers[4].SetAllLedsColor(0,0,0);
            //LedLayers[5].SetAllLedsColor(0.5f,0,0.7f);
            //for (int index = 0; index < LedLayers.Length; index++)
            //{
            //     LedLayers[index].WriteBufferFromLeds(layers[index].LayerBuffer12Bit);
            //}
        }



        public void Start()
        {
            while (true)
            {
                if (animation.NextFrameIsReady)
                {
                    for (int i = 0; i < layers.Length; i++)
                    {
                        Helper.SwapBuffers(ref animation.Frame.LayerBuffers12Bit[i],ref layers[i].LayerBuffer12Bit);
                    }
                    animation.NextFrameIsReady = false;
                }
                // setcurrentlayerbuffers

               Helper.CycleLayers(layers,tlc5940);
            }
        }

       
    }

}

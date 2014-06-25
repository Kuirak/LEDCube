using System;
using Microsoft.SPOT;

namespace LEDCube
{
    public class RemoteCube
    {
        private readonly Tlc5940 tlc5940;
        private readonly Layer[] layers;
        private readonly Layer[] cachedLayers;
        private bool[] newData;

        public RemoteCube(Tlc5940 tlc5940)
        {
            this.tlc5940 = tlc5940;
            layers = Helper.CreateLayers();
            cachedLayers = Helper.CreateLayers();
            newData=new bool[6];

        }

        public void Start()
        {
            while (true)
            {
                for (int i = 0; i < layers.Length; i++)
                {
                    if (newData[i])
                    {
                        Helper.SwapBuffers(ref cachedLayers[i].LayerBuffer12Bit, ref layers[i].LayerBuffer12Bit);
                        newData[i] = false;
                    }
                }
               
                Helper.CycleLayers(layers,tlc5940);
            }
        }

        public void NewLayerData(int layerIdx, ref byte[] data)
        {
            Debug.Print("New layer data: "+layerIdx);
            Helper.SwapBuffers(ref data, ref cachedLayers[layerIdx].LayerBuffer12Bit);
            newData[layerIdx] = true;
            
        }
    }
}

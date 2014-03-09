using System;
using Microsoft.SPOT;

namespace LEDCube
{
    public class Frame
    {
        private byte[][] LayerBuffers12Bit;
        public bool IsReady = false;

        public Frame()
        {
            LayerBuffers12Bit = new byte[Config.SideLength][];
                                
            for (int i = 0; i <LayerBuffers12Bit.Length; i++)
            {
                LayerBuffers12Bit[i] = Tlc5940.CreateBuffer(Config.TlcChannelCount);
            }
        }

        

    }
}

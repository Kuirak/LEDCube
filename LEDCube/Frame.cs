using System;
using Microsoft.SPOT;

namespace LEDCube
{
    public class Frame
    {
        public byte[][] LayerBuffers12Bit;
        public bool IsReady = false;

        public Frame()
        {
            LayerBuffers12Bit = new byte[Config.SideLength][];
                                
            for (int i = 0; i <LayerBuffers12Bit.Length; i++)
            {
                LayerBuffers12Bit[i] = Tlc5940.CreateBuffer(Config.TlcChannelCount);
            }
        }

        //public byte[] this[int index]
        //{
        //    get { return LayerBuffers12Bit[index]; }
        //    set { LayerBuffers12Bit[index] =value; }
        //}
    }
}

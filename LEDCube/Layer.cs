using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace LEDCube
{
    public class Layer
    {
        public readonly OutputPort Port;
        public byte[] LayerBuffer12Bit;

        public Layer(OutputPort port)
        {
            Port = port;
            LayerBuffer12Bit = Tlc5940.CreateBuffer(Config.TlcChannelCount);
           
        }

        public void On()
        {
            Port.Write(true);
        }

        public void Off()
        {
            Port.Write(false);
        }
        
    }
}

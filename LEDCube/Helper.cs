using System;
using System.Threading;
using Microsoft.SPOT;

namespace LEDCube
{
   public static class Helper
    {
       public static void SwapBuffers(ref byte[] newBuffer, ref byte[] currentBuffer)
       {
          
           var old = currentBuffer;
           currentBuffer = newBuffer;
           newBuffer = old;
       }

       public static Layer[] CreateLayers()
       {
            var layers = new Layer[Config.SideLength];

           for (int i = 0; i < layers.Length; i++)
           {
               layers[i] = new Layer(Config.LayerPorts[i]);

           }
           return layers;
       }

       public static void CycleLayers(Layer[] layers,Tlc5940 tlc5940)
       {
           for (int index = 0; index < layers.Length; index++)
           {
               var layer = layers[index];
               tlc5940.PushBuffer(ref layer.LayerBuffer12Bit);

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

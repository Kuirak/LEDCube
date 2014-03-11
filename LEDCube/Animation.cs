using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;

namespace LEDCube
{
    public class Animation
    {
        private readonly LedLayer[] LedLayers;
        public bool NextFrameIsReady;
        public Frame Frame;

        public Animation()
        {
            LedLayers = new LedLayer[Config.SideLength];
            for (int i = 0; i < LedLayers.Length; i++)
            {
                LedLayers[i]= new LedLayer(i);
            }
            Frame = new Frame();
        }


        public void Start()
        {
            var red = 1f;
            var green = 0f;
            var blue = 0f;
            while (true)
            {
                if (WaitForFrameToBeApplied()) continue;
                red = red.Repeat(0.01f, 0.1f, 1);
                //green = green.Repeat(0.2f, 0, 1);
                //blue = blue.Repeat(-0.1f, 0, 1);
                for (int i = 0; i < LedLayers.Length; i++)
                {
                    var layer = LedLayers[i];
                    
                    layer.SetAllLedsColor(red, green, blue);
                    Thread.Sleep(2);
                    layer.WriteBufferFromLeds(ref Frame.LayerBuffers12Bit[i]);
                    Thread.Sleep(2);
                }
                Thread.Sleep(50);
                NextFrameIsReady = true;
            }
        }

        private bool WaitForFrameToBeApplied()
        {
            if (NextFrameIsReady)
            {
                Thread.Sleep(16);
                return true;
            }
            return false;
        }

        private void WriteBuffer()
        {
            for (int index = 0; index < LedLayers.Length; index++)
            {
                LedLayers[index].WriteBufferFromLeds(ref Frame.LayerBuffers12Bit[index]);
                Thread.Sleep(1);
            }
        }

        public void Rain()
        {
            Debug.Print("Rain Animation Started");
            var rand = new Random();
            var newDropTimer = 0;
            const int newDropTime = 2;
            var drops = new ArrayList ();
            var remainingDrops = new ArrayList() ;
           

            while (true)
            {
                if (WaitForFrameToBeApplied()) continue;
               
                foreach (Led drop in drops)
                {
                    drop.Off();
                    var layerindex= drop.layer.index;
                    layerindex--;
                    if (layerindex <0)
                    {
                        continue;
                    }
                     var led =LedLayers[layerindex].Leds[drop.index];
                     led.Set(0, 0.1f, 1);
                     remainingDrops.Add(led);
                    Thread.Sleep(1);
                }
                drops.Clear();
                var oldDrops = drops;
                drops = remainingDrops;
                remainingDrops = oldDrops;

                newDropTimer ++;
                if (newDropTimer >= newDropTime)
                {
                    var newDropCount = rand.Next(10).Clamp(2,10);
                    
                    newDropTimer = 0;
                    for (int i = 0; i < newDropCount; i++)
                    {
                        var x = rand.Next((int)Config.SideLength);
                        Thread.Sleep(1);
                        var y = rand.Next((int)Config.SideLength);
                        Thread.Sleep(1);
                        var led = LedLayers[Config.SideLength - 1].GetLed(x, y);
                        Thread.Sleep(1);
                        led.Set(0, 0.1f, 1);
                        drops.Add(led);
                        
                    }
                    
                }
                
                WriteBuffer();
                NextFrameIsReady = true;
            }
        }

    }
}

using System;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;



namespace LEDCube
{
    public class Program
    {
        static readonly SerialPortHelper SerialPortHelper = new SerialPortHelper();
    
        public static void Main()
        {
            
            
           
            var button = new InterruptPort(Pins.ONBOARD_SW1, true, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
            var led=new OutputPort(Pins.ONBOARD_LED, false);

            new Thread(() =>
                   {
                       while (true)
                       {


                           led.Write(true);
                           Thread.Sleep(500);
                           led.Write(false);
                           Thread.Sleep(200);
                       }

                   }).Start();
            var tlc5940 = new Tlc5940();
            
            //var anim = new Animation();
            //var cube = new Cube(tlc5940,anim);
            //ThreadStart animThreadStart = anim.Rain;
           // var animThread = new Thread(animThreadStart) {Priority = ThreadPriority.Lowest};
            var cube = new RemoteCube(tlc5940);
            ThreadStart tlcThreadStart = cube.Start;
            var tlcThread = new Thread(tlcThreadStart) {Priority = ThreadPriority.Highest};
            tlcThread.Start();
            //animThread.Start();


            new Thread(() =>
            {
                //SerialPortHelper.PrintLine("");
                //SerialPortHelper.PrintLine("r");
                var layer = Tlc5940.CreateBuffer(Config.TlcChannelCount);

                while (true)
                {
                    Thread.Sleep(1);
                    string line = SerialPortHelper.ReadLine();
                    
                    if (line.Length > 0)
                    {
                        if (line.Length == 1)
                        {
                            if (line == "\t")
                            {
                                continue;
                            }
                            var layerIdx = Convert.ToInt32(line);
                            bool readLayer;
                            do
                            {
                                readLayer = SerialPortHelper.ReadLayer(ref layer);
                                Thread.Sleep(1);

                            } while (!readLayer);
                            cube.NewLayerData(layerIdx, ref layer);
                        }

                        //if (line == "r")
                        //{
                        //    SerialPortHelper.PrintLine("s");
                        //    continue;
                        //}
                        //if (line == "f")
                        //{
                        //    SerialPortHelper.PrintLine("f");
                        //    continue;
                        //}
                        //SerialPortHelper.PrintLine(line);
                        //if (line == "e")
                        //    SerialPortHelper.PrintLine("n");
                    }
                }
            }).Start();
          


        }

        
    }
}
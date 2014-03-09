using System;
using Microsoft.SPOT;



namespace LEDCube
{
    public class Led
    {
        /// <summary>
        /// Validate that the given value is between 0 and 100
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint Validate(uint value)
        {
            return value.Clamp(0, 100);
        }

        /// <summary>
        /// Converts to Tlc Values where  100% are 4095
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint ConvertToTlc(float value)
        {
            return (uint)(4095 * value / 100.0);
        }


        private uint red;
        public uint Red
        {
            get { return red; }
            set { red = ConvertToTlc(Validate(value)); }
        } 

        private uint green;
        public uint Green
        {
            get { return green; }
            set { green = ConvertToTlc(Validate(value)); }
        } 

        private uint blue;
        public uint Blue
        {
            get { return blue; }
            set { blue = ConvertToTlc(Validate(value)); }
        }

        /// <summary>
        /// Turns on all colors to max
        /// </summary>
        public void Off()
        {
           Set(0,0,0);
          
        }

        /// <summary>
        /// Turns on all colors to max
        /// </summary>
        public void On()
        {
            Set(100, 100, 100);
            
        }
        

        /// <summary>
        /// Sets the color
        /// </summary>
        /// <param name="r">Red Color value</param>
        /// <param name="g">Green Color value</param>
        /// <param name="b">Blue Color value</param>
        public void Set(uint r, uint g, uint b)
        {
            Red = r;
            Green = g;
            Blue = b;

        }

        //convert to computed values
        public readonly uint redIndex;
        public readonly uint greenIndex;
        public readonly uint blueIndex;

        public Led(uint startIndex)
        {
            redIndex = startIndex;
            greenIndex = (redIndex + 1);
            blueIndex = (greenIndex + 1);
            
        }
    }
}

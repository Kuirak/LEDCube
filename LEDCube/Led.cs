using System;
using Microsoft.SPOT;



namespace LEDCube
{
    public struct Led
    {
        /// <summary>
        /// Validate that the given value is between 0 and 100
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Validate(float value)
        {
            return value.Clamp(0, 1);
        }

        /// <summary>
        /// Converts to Tlc Values where  100% are 4095
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint ConvertToTlc(float value)
        {
            return (uint)(4095 * value);
        }


        private uint red;
        public uint Red
        {
            get { return red; }
           private set { red = value; }
        }

        private uint green;
        public uint Green
        {
            get { return green; }
           private set { green = value; }
        }

        private uint blue;
        public uint Blue
        {
            get { return blue; }
            private set { blue =value; }
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
            Set(1, 1, 1);
            
        }
        

        /// <summary>
        /// Sets the color
        /// </summary>
        /// <param name="r">Red Color value</param>
        /// <param name="g">Green Color value</param>
        /// <param name="b">Blue Color value</param>
        public void Set(float r, float g, float b)
        {
            Red = ConvertToTlc(Validate(r));
            Green = ConvertToTlc(Validate(g));
            Blue = ConvertToTlc(Validate(b));

        }

        //convert to computed values
        public readonly uint RedIndex;
        public uint GreenIndex{get { return RedIndex +1; }}
        public uint BlueIndex { get { return RedIndex + 2; } }

        public Led(uint startIndex)
        {
            RedIndex = startIndex;
            red = 0;
            green = 0;
            blue = 0;
        }
    }
}

using System;
using Microsoft.SPOT;

namespace LEDCube
{
    public static class Extensions
    {
        public static float Clamp(this float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        } 
        
        public static int Clamp(this int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static float Repeat(this float value,float step, float start, float end)
        {
            value += step;
            return value < start ? end : (value > end ? start : value);
        }

        public static byte[] Reset(this byte[] value )
        {
            for (int i = 0; i < value.Length; i++)
            {
                value[i] = 0;
            }
            return value;
        }

    }
}

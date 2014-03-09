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

        public static uint Repeat(this uint value, uint start, uint end)
        {
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

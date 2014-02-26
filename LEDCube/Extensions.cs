using System;
using Microsoft.SPOT;

namespace LEDCube
{
    public static class Extensions
    {
        public static uint Clamp(this uint value, uint min, uint max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static uint Repeat(this uint value, uint start, uint end)
        {
            return value < start ? end : (value > end ? start : value);
        }
    }
}

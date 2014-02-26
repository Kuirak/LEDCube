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
    }
}

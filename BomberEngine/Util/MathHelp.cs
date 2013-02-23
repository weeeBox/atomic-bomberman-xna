﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Util
{
    public sealed class MathHelp
    {
        private static Random random;

        public static int Sign(float value)
        {
            return value > 0 ? 1 : (value < 0 ? -1 : 0);
        }

        public static int Sign(int value)
        {
            return value > 0 ? 1 : (value < 0 ? -1 : 0);
        }

        public static float Abs(float value)
        {
            return value < 0 ? -value : value;
        }

        public static int Abs(int value)
        {
            return value < 0 ? -value : value;
        }

        public static int NextInt(int maxValue)
        {
            return GetRandom().Next(maxValue);
        }

        public static int NextInt(int minValue, int maxValue)
        {
            return GetRandom().Next(minValue, maxValue);
        }

        public static float NextFloat()
        {
            return (float) GetRandom().NextDouble();
        }

        private static Random GetRandom()
        {
            if (random == null)
            {
                random = new Random();
            }
            return random;
        }

        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }
    }
}
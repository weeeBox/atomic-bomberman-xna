using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Util
{
    public sealed class MathHelp
    {
        private static Random random;

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
    }
}

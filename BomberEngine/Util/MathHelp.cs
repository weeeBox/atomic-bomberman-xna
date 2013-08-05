using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Util
{
    public sealed class MathHelp
    {
        private static Random s_random;
        private static int s_seed;

        public static void InitRandom()
        {
            InitRandom((int)DateTime.Now.Ticks);
        }

        public static void InitRandom(int seed)
        {
            s_seed = seed;
            s_random = new Random(s_seed);
        }

        public static int GetRandomSeed()
        {
            return s_seed;
        }

        public static int NextInt(int maxValue)
        {
            return s_random.Next(maxValue);
        }

        public static int NextInt(int minValue, int maxValue)
        {
            return s_random.Next(minValue, maxValue);
        }

        public static float NextFloat()
        {
            return (float)s_random.NextDouble();
        }
    }
}

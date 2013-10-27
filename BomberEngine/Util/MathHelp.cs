using System;

namespace BomberEngine
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

        public static float NextFloat(float maxValue)
        {
            return maxValue * NextFloat();
        }

        public static float NextFloat(float minValue, float maxValue)
        {
            return minValue + NextFloat() * (maxValue - minValue);
        }

        public static double NextDouble()
        {
            return s_random.NextDouble();
        }

        public static double NextDouble(double maxValue)
        {
            return maxValue * NextDouble();
        }

        public static double NextDouble(double minValue, double maxValue)
        {
            return minValue + NextDouble() * (maxValue - minValue);
        }

        public static int ForceRange(int x, int min, int max)
        {
            Debug.Assert(min <= max);
            return Math.Max(min, Math.Min(x, max));
        }
    }
}

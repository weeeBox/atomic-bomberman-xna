using System;

namespace BomberEngine
{
    public class ArrayUtils
    {
        public static void Clear(Array array)
        {
            Clear(array, 0, array.Length);
        }

        public static void Clear(Array array, int index, int length)
        {
            Array.Clear(array, index, length);
        }

        public static void Shuffle<T>(T[] array)
        {
            Shuffle(array, array.Length);
        }

        public static void Shuffle<T>(T[] array, int size)
        {
            int n = size;
            while (n > 1)
            {
                n--;
                int k = MathHelp.NextInt(n + 1);
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }
    }
}

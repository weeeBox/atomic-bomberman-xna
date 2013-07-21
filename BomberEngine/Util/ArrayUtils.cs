using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Util
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Debugging
{
    public class Log
    {
        public static void d(Object format, params Object[] args)
        {
            d(format != null ? format.ToString() : "null", args);
        }

        public static void d(String format, params Object[] args)
        {
            String message = StringUtils.TryFormat(format, args);
            Console.WriteLine(message);
        }
    }
}

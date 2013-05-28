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
            String message = Format(format, args);
            Console.WriteLine("D/" + message);
        }

        public static void e(Object format, params Object[] args)
        {
            String message = Format(format, args);
            Console.WriteLine("E/" + message);
        }

        public static void error(Exception ex, Object format, params Object[] args)
        {
            String message = Format(format, args);
            Console.WriteLine(message);
            Console.WriteLine(ex.StackTrace);
        }

        private static String Format(Object format, params Object[] args)
        {
            return format != null ? StringUtils.TryFormat(format.ToString(), args) : null;
        }
    }
}

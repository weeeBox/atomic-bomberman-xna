using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;
using System.Diagnostics;

namespace BomberEngine.Debugging
{
    public class Log
    {
        [Conditional("DEBUG")]
        public static void d(Object format, params Object[] args)
        {
            String message = Format(format, args);
            Console.WriteLine("D/" + message);
        }

        [Conditional("DEBUG")]
        public static void d(bool condition, Object format, params Object[] args)
        {
            if (condition)
            {
                d(format, args);
            }
        }

        [Conditional("DEBUG")]
        public static void e(Object format, params Object[] args)
        {
            String message = Format(format, args);
            Console.WriteLine("E/" + message);
        }

        [Conditional("DEBUG")]
        public static void i(Object format, params Object[] args)
        {
            String message = Format(format, args);
            Console.WriteLine("I/" + message);
        }

        [Conditional("DEBUG")]
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

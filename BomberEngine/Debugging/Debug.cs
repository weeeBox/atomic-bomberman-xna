using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Debugging
{
    public class Debug
    {
        public static void CheckArgument(bool condition, String format, params Object[] args)
        {
            if (!condition)
            {
                String message = StringUtils.TryFormat(format, args);
                throw new ArgumentException(message);
            }
        }

        public static void CheckArgumentNotNull(String name, Object reference)
        {
            CheckArgument(reference != null, "'{0}' is null", name);
        }

        public static void CheckArgumentRange(String name, int argument, int min, int max)
        {
            CheckArgument(argument >= min && argument < max, "'{0}' {1} out of range {2}..{3}", name, argument, min, max - 1);
        }

        public static void Assert(bool condition, String format, params Object[] args)
        {
            System.Diagnostics.Debug.Assert(condition, "Assertion failed", format, args);
        }

        public static void Assert(bool condition, String message)
        {   
            System.Diagnostics.Debug.Assert(condition, "Assertion failed", message);
        }
    }
}

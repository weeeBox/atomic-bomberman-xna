using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;
using System.Diagnostics;

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

        [Conditional("DEBUG")]
        public static void Assert(bool condition, String format, params Object[] args)
        {
            System.Diagnostics.Debug.Assert(condition, "Assertion failed", format, args);
        }

        [Conditional("DEBUG")]
        public static void Assert(bool condition, String message)
        {   
            System.Diagnostics.Debug.Assert(condition, "Assertion failed", message);
        }

        [Conditional("DEBUG")]
        public static void Assert(bool condition)
        {
            System.Diagnostics.Debug.Assert(condition, "Assertion failed");
        }

        [Conditional("DEBUG")]
        public static void AssertContains<T>(ICollection<T> collection, T element)
        {
            System.Diagnostics.Debug.Assert(collection.Contains(element), "Assertion failed");
        }
            
        [Conditional("DEBUG")]
        public static void AssertNotContains<T>(ICollection<T> collection, T element)
        {
            System.Diagnostics.Debug.Assert(!collection.Contains(element), "Assertion failed");
        }

        [Conditional("DEBUG")]
        public static void Fail(String format, params Object[] args)
        {
            System.Diagnostics.Debug.Assert(false, "Failed!", format, args);
        }
    }
}

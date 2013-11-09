using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BomberEngine
{
    public static class Assert
    {
        [Conditional("DEBUG")]
        public static void True(bool condition, String format, params Object[] args)
        {
            System.Diagnostics.Debug.Assert(condition, "Assertion failed", format, args);
        }

        [Conditional("DEBUG")]
        public static void True(bool condition, String message)
        {
            System.Diagnostics.Debug.Assert(condition, "Assertion failed", message);
        }

        [Conditional("DEBUG")]
        public static void True(bool condition)
        {
            System.Diagnostics.Debug.Assert(condition, "Assertion failed");
        }

        [Conditional("DEBUG")]
        public static void False(bool condition, String format, params Object[] args)
        {
            System.Diagnostics.Debug.Assert(!condition, "Assertion failed", format, args);
        }

        [Conditional("DEBUG")]
        public static void False(bool condition, String message)
        {
            System.Diagnostics.Debug.Assert(!condition, "Assertion failed", message);
        }

        [Conditional("DEBUG")]
        public static void False(bool condition)
        {
            System.Diagnostics.Debug.Assert(!condition, "Assertion failed");
        }

        [Conditional("DEBUG")]
        public static void NotNull(Object o)
        {
            System.Diagnostics.Debug.Assert(o != null, "Assertion failed: object is null");
        }

        [Conditional("DEBUG")]
        public static void Null(Object o)
        {
            System.Diagnostics.Debug.Assert(o == null, "Assertion failed: object is not null");
        }

        [Conditional("DEBUG")]
        public static void Contains<T>(ICollection<T> collection, T element)
        {
            System.Diagnostics.Debug.Assert(collection.Contains(element), "Assertion failed");
        }

        [Conditional("DEBUG")]
        public static void NotContains<T>(ICollection<T> collection, T element)
        {
            System.Diagnostics.Debug.Assert(!collection.Contains(element), "Assertion failed");
        }

        [Conditional("DEBUG")]
        public static void Range(int index, Array array)
        {
            System.Diagnostics.Debug.Assert(index >= 0 && index < array.Length, "Assertion failed", "Index {0} is out of range {1}..{2}", index, 0, array.Length);
        }

        [Conditional("DEBUG")]
        public static void Range(int index, int min, int max)
        {
            System.Diagnostics.Debug.Assert(index >= min && index < max, "Assertion failed", "Index {0} is out of range {1}..{2}", index, min, max);
        }

        [Conditional("DEBUG")]
        public static void AreEqual(Object o1, Object o2)
        {
            System.Diagnostics.Debug.Assert(o1 == o2, "Assertion failed", "{0} != {1}", o1, o2);
        }

        [Conditional("DEBUG")]
        public static void Fail(String format, params Object[] args)
        {
            System.Diagnostics.Debug.Assert(false, "Failed!", format, args);
        }
    }
}

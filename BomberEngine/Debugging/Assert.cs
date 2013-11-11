using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BomberEngine
{
    public static class Assert
    {
        [Conditional("DEBUG")]
        public static void IsTrue(bool condition, String format, params Object[] args)
        {
            System.Diagnostics.Debug.Assert(condition, "Assertion failed", format, args);
        }

        [Conditional("DEBUG")]
        public static void IsTrue(bool condition, String message)
        {
            System.Diagnostics.Debug.Assert(condition, "Assertion failed", message);
        }

        [Conditional("DEBUG")]
        public static void IsTrue(bool condition)
        {
            System.Diagnostics.Debug.Assert(condition, "Assertion failed");
        }

        [Conditional("DEBUG")]
        public static void IsFalse(bool condition, String format, params Object[] args)
        {
            System.Diagnostics.Debug.Assert(!condition, "Assertion failed", format, args);
        }

        [Conditional("DEBUG")]
        public static void IsFalse(bool condition, String message)
        {
            System.Diagnostics.Debug.Assert(!condition, "Assertion failed", message);
        }

        [Conditional("DEBUG")]
        public static void IsFalse(bool condition)
        {
            System.Diagnostics.Debug.Assert(!condition, "Assertion failed");
        }

        [Conditional("DEBUG")]
        public static void IsNotNull(Object o)
        {
            System.Diagnostics.Debug.Assert(o != null, "Assertion failed: object is null");
        }

        [Conditional("DEBUG")]
        public static void IsNull(Object o)
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
        public static void IsIndex(int index, Array array)
        {
            System.Diagnostics.Debug.Assert(index >= 0 && index < array.Length, "Assertion failed", "Index {0} is out of range {1}..{2}", index, 0, array.Length);
        }

        [Conditional("DEBUG")]
        public static void IsRange(int index, int min, int max)
        {
            System.Diagnostics.Debug.Assert(index >= min && index <= max, "Assertion failed", "Index {0} is out of range {1}..{2}", index, min, max);
        }

        [Conditional("DEBUG")]
        public static void IsRange(int index, uint min, uint max)
        {
            System.Diagnostics.Debug.Assert(index >= min && index <= max, "Assertion failed", "Index {0} is out of range {1}..{2}", index, min, max);
        }

        [Conditional("DEBUG")]
        public static void AreEqual(int expected, int actual)
        {
            System.Diagnostics.Debug.Assert(expected == actual, "Assertion failed", "{0} != {1}", expected, actual);
        }

        [Conditional("DEBUG")]
        public static void AreEqual(float expected, float actual)
        {
            System.Diagnostics.Debug.Assert(expected == actual, "Assertion failed", "{0} != {1}", expected, actual);
        }

        [Conditional("DEBUG")]
        public static void AreEqual(Object o1, Object o2)
        {
            System.Diagnostics.Debug.Assert(o1 == o2, "Assertion failed", "{0} != {1}", o1, o2);
        }

        [Conditional("DEBUG")]
        public static void IsInstance<T>(Object obj)
        {
            System.Diagnostics.Debug.Assert(obj is T, "Assertion failed"); // TODO: message
        }

        [Conditional("DEBUG")]
        public static void Fail(String format, params Object[] args)
        {
            System.Diagnostics.Debug.Assert(false, "Failed!", format, args);
        }
    }
}

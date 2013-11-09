using System;
using BomberEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BomberEngineTests
{
    using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

    [TestClass]
    public class BitArrayTests
    {
        [TestMethod]
        public void TestEmpty()
        {
            BitArray array = new BitArray(10);
            Assert.AreEqual(10, array.length);

            for (int i = 0; i < array.length; ++i)
            {
                Assert.AreEqual(false, array[i]);
            }
        }

        [TestMethod]
        public void TestData()
        {
            BitArray array = new BitArray(10);

            bool[] flags = new bool[10];
            for (int i = 0; i < flags.Length; ++i)
            {
                array[i] = flags[i] = i % 2 != 0;
            }

            for (int i = 0; i < flags.Length; ++i)
            {
                Assert.AreEqual(flags[i], array[i]);
            }
        }

        [TestMethod]
        public void TestInvert()
        {
            BitArray array = new BitArray(1);

            int i = 0;
            array[i] = !array[i];

            Assert.IsTrue(array[i]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidArgument()
        {
            new BitArray(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidArgument2()
        {
            new BitArray(BitArray.MaxLength + 1);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestInvalidIndex()
        {
            BitArray array = new BitArray(10);
            array[-1] = true;
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestInvalidIndex2()
        {
            BitArray array = new BitArray(10);
            array[10] = true;
        }
    }
}



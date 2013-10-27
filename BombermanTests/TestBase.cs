using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BombermanTests
{   
    public class TestBase
    {
        protected void AssertResult<T>(List<T> actual, params T[] expected)
        {
            Assert.AreEqual(actual.Count, expected.Length);

            for (int i = 0; i < actual.Count; ++i)
            {
                Assert.AreEqual(actual[i], expected[i]);
            }
        }

        protected void AssertResult<T>(T[] actual, params T[] expected)
        {
            Assert.AreEqual(actual.Length, expected.Length);

            for (int i = 0; i < actual.Length; ++i)
            {
                Assert.AreEqual(actual[i], expected[i]);
            }
        }
    }
}

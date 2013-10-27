using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BomberEngine;

namespace BomberEngineTests
{
    [TestClass]
    public class CConsoleTests
    {
        [TestMethod]
        public void TestArgs1()
        {
            String cmd = @"cmd";

            DummyConsole c = new DummyConsole();
            AssertResult(c.extractArgs(cmd), "cmd");
        }

        [TestMethod]
        public void TestArgs2()
        {
            String cmd = @"cmd arg1";

            DummyConsole c = new DummyConsole();
            AssertResult(c.extractArgs(cmd), "cmd", "arg1");
        }

        [TestMethod]
        public void TestArgs3()
        {
            String cmd = @"cmd arg1 arg2";

            DummyConsole c = new DummyConsole();
            AssertResult(c.extractArgs(cmd), "cmd", "arg1", "arg2");
        }

        [TestMethod]
        public void TestArgs4()
        {
            String cmd = @"cmd arg1 arg2 arg3";

            DummyConsole c = new DummyConsole();
            AssertResult(c.extractArgs(cmd), "cmd", "arg1", "arg2", "arg3");
        }

        [TestMethod]
        public void TestArgs5()
        {
            String cmd = @"cmd ";

            DummyConsole c = new DummyConsole();
            AssertResult(c.extractArgs(cmd), "cmd");
        }

        [TestMethod]
        public void TestArgs6()
        {
            String cmd = @"cmd  ";

            DummyConsole c = new DummyConsole();
            AssertResult(c.extractArgs(cmd), "cmd");
        }

        [TestMethod]
        public void TestArgs7()
        {
            String cmd = @"cmd   ";

            DummyConsole c = new DummyConsole();
            AssertResult(c.extractArgs(cmd), "cmd");
        }

        [TestMethod]
        public void TestArgs8()
        {
            String cmd = @"cmd  arg1";

            DummyConsole c = new DummyConsole();
            AssertResult(c.extractArgs(cmd), "cmd", "arg1");
        }

        [TestMethod]
        public void TestArgs9()
        {
            String cmd = @"cmd  arg1  arg2";

            DummyConsole c = new DummyConsole();
            AssertResult(c.extractArgs(cmd), "cmd", "arg1", "arg2");
        }

        [TestMethod]
        public void TestArgs10()
        {
            String cmd = @"cmd  arg1  arg2     arg3 ";

            DummyConsole c = new DummyConsole();
            AssertResult(c.extractArgs(cmd), "cmd", "arg1", "arg2", "arg3");
        }

        [TestMethod]
        public void TestArgs11()
        {
            String cmd = "cmd \"\"";

            DummyConsole c = new DummyConsole();
            AssertResult(c.extractArgs(cmd), "cmd", "");
        }

        [TestMethod]
        public void TestArgs12()
        {
            String cmd = "cmd \"arg 1\"";

            DummyConsole c = new DummyConsole();
            AssertResult(c.extractArgs(cmd), "cmd", "arg 1");
        }

        [TestMethod]
        public void TestArgs13()
        {
            String cmd = "cmd \"arg 1\" \"arg 2 \" \" arg 3 \"";

            DummyConsole c = new DummyConsole();
            AssertResult(c.extractArgs(cmd), "cmd", "arg 1", "arg 2 ", " arg 3 ");
        }

        [TestMethod]
        public void TestArgs14()
        {
            String cmd = "cmd arg1 \"arg2\" arg3";

            DummyConsole c = new DummyConsole();
            AssertResult(c.extractArgs(cmd), "cmd", "arg1", "arg2", "arg3");
        }

        [TestMethod]
        public void TestArgs15()
        {
            String cmd = "cmd arg1 \"arg2 arg3";

            DummyConsole c = new DummyConsole();
            Assert.IsNull(c.extractArgs(cmd), "cmd");
        }

        [TestMethod]
        public void TestArgs16()
        {
            String cmd = "cmd arg1 arg2 arg3\"";

            DummyConsole c = new DummyConsole();
            Assert.IsNull(c.extractArgs(cmd), "cmd");
        }

        [TestMethod]
        public void TestArgs17()
        {
            String cmd = "cmd \"arg1 arg2\"\"\" arg3\"";

            DummyConsole c = new DummyConsole();
            Assert.IsNull(c.extractArgs(cmd), "cmd");
        }

        private void AssertResult(List<String> actual, params String[] expected)
        {   
            Assert.AreEqual(actual.Count, expected.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                Assert.AreEqual(actual[i], expected[i]);
            }
        }
    }

    public class DummyConsole : CConsole
    {
        public DummyConsole()
            : base(0, 0)
        {
        }

        public new List<String> extractArgs(String str)
        {
            return base.extractArgs(str);
        }
    }
}

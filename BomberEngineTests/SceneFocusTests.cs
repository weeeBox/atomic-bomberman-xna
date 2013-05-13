using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BomberEngine.Game;
using BomberEngine.Core.Visual.UI;

namespace BomberEngineTests
{
    [TestClass]
    public class SceneFocusTests
    {
        [TestMethod]
        public void TestDefaultFocus1()
        {
            Screen screen = new Screen(0, 0);

            FocusableView f1 = new FocusableView("f1");
            Assert.IsFalse(f1.focused);

            View container = new View();
            container.AddChild(f1);

            screen.SetRootView(container);

            Assert.IsTrue(f1.focused);
            Assert.AreEqual(screen.focusedView, f1);
        }

        [TestMethod]
        public void TestDefaultFocus2()
        {
            Screen screen = new Screen(0, 0);

            FocusableView f1 = new FocusableView("f1");
            Assert.IsFalse(f1.focused);

            View container1 = new View();
            container1.AddChild(f1);

            View container2 = new View();
            container2.AddChild(container1);

            screen.SetRootView(container2);

            Assert.IsTrue(f1.focused);
            Assert.AreEqual(screen.focusedView, f1);
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    #region Test classes

    class TestView : View
    {
        public String name;

        public TestView(String name, bool focusable)
        {
            this.name = name;
            this.focusable = focusable;
        }
    }

    class FocusableView : TestView
    {
        public FocusableView(String name)
            : base(name, true)
        {
        }
    }

    class NotFocusableView : TestView
    {
        public NotFocusableView(String name)
            : base(name, false)
        {
        }
    }

    #endregion
}

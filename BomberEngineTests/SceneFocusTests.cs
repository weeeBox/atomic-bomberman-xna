using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BomberEngine.Game;
using BomberEngine.Core.Visual.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Visual;

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
            container.AddView(f1);

            //screen.SetRootView(container);

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
            container1.AddView(f1);

            View container2 = new View();
            container2.AddView(container1);

            //screen.SetRootView(container2);

            Assert.IsTrue(f1.focused);
            Assert.AreEqual(screen.focusedView, f1);
        }

        [TestMethod]
        public void TestDefaultFocus3()
        {
            Screen screen = new Screen(0, 0);

            FocusableView f1 = new FocusableView("f1");
            Assert.IsFalse(f1.focused);

            View container1 = new View();
            container1.AddView(f1);

            View container2 = new View();
            container2.AddView(container1);

            View container3 = new View();
            container3.AddView(container2);

            //screen.SetRootView(container3);

            Assert.IsTrue(f1.focused);
            Assert.AreEqual(screen.focusedView, f1);
        }

        [TestMethod]
        public void TestDefaultFocus4()
        {
            Screen screen = new Screen(0, 0);

            NotFocusableView n1 = new NotFocusableView("n1");
            Assert.IsFalse(n1.focused);

            NotFocusableView n2 = new NotFocusableView("n2");
            Assert.IsFalse(n2.focused);

            FocusableView f1 = new FocusableView("f1");
            Assert.IsFalse(f1.focused);

            FocusableView f2 = new FocusableView("f3");
            Assert.IsFalse(f1.focused);

            View container1 = new View();
            container1.AddView(n1);
            container1.AddView(n2);
            container1.AddView(f1);

            View container2 = new View();

            View container3 = new View();
            container3.AddView(f2);

            View rootView = new View();
            rootView.AddView(container1);
            rootView.AddView(container2);
            rootView.AddView(container3);

            //screen.SetRootView(rootView);

            Assert.IsTrue(f1.focused);
            Assert.AreEqual(screen.focusedView, f1);
        }

        [TestMethod]
        public void TestDefaultFocus5()
        {
            Screen screen = new Screen(0, 0);

            NotFocusableView n1 = new NotFocusableView("n1");
            Assert.IsFalse(n1.focused);

            NotFocusableView n2 = new NotFocusableView("n2");
            Assert.IsFalse(n2.focused);

            FocusableView f1 = new FocusableView("f1");
            Assert.IsFalse(f1.focused);

            FocusableView f2 = new FocusableView("f3");
            Assert.IsFalse(f1.focused);

            View container1 = new NotFocusableView("c1");
            container1.AddView(n1);
            container1.AddView(n2);
            container1.AddView(f1);

            View container2 = new NotFocusableView("c2");

            View container3 = new NotFocusableView("c3");
            container3.AddView(f2);

            View rootView = new NotFocusableView("root");
            rootView.AddView(container1);
            rootView.AddView(container2);
            rootView.AddView(container3);

            //screen.SetRootView(rootView);

            Assert.IsTrue(f1.focused);
            Assert.AreEqual(screen.focusedView, f1);

            screen.OnKeyPressed(Keys.Down);

            Assert.IsTrue(f2.focused);
            Assert.AreEqual(screen.focusedView, f2);

            Assert.IsFalse(f1.focused);
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

        public override string ToString()
        {
            return GetType().Name + ": " + name;
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

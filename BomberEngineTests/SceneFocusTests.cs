using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BomberEngine.Game;
using BomberEngine.Core.Visual.UI;

using BomberEngine.Core.Visual;
using BomberEngine.Core.Events;
using BomberEngine.Core.Input;

namespace BomberEngineTests
{
    [TestClass]
    public class SceneFocusTests
    {
        [TestMethod]
        public void TestDefaultFocus1()
        {
            Screen screen = new Screen(0, 0);

            FocusableView view = new FocusableView("f1");
            screen.AddView(view);

            screen.Start();

            Assert.IsTrue(view.focused);
        }

        [TestMethod]
        public void TestDefaultFocus2()
        {
            Screen screen = new Screen(0, 0);

            FocusableView view = new FocusableView("f1");
            View container = new View();
            container.AddView(view);

            screen.AddView(container);

            screen.Start();

            Assert.IsTrue(view.focused);
        }

        [TestMethod]
        public void TestDefaultFocus3()
        {
            Screen screen = new Screen(0, 0);

            screen.AddView(new View());

            FocusableView view = new FocusableView("f1");
            View container = new View();
            container.AddView(view);

            screen.AddView(container);
            screen.Start();

            Assert.IsTrue(view.focused);
        }

        [TestMethod]
        public void TestDefaultFocus4()
        {
            Screen screen = new Screen(0, 0);

            FocusableView f1 = new FocusableView("f1");

            View container1 = new View();
            container1.AddView(f1);

            View container2 = new View();
            container2.AddView(container1);

            screen.AddView(container2);
            screen.Start();

            Assert.IsTrue(f1.focused);
            Assert.AreEqual(screen.focusedView, f1);
        }

        [TestMethod]
        public void TestDefaultFocus5()
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

            screen.AddView(container3);
            screen.Start();

            Assert.IsTrue(f1.focused);
            Assert.AreEqual(screen.focusedView, f1);
        }

        [TestMethod]
        public void TestDefaultFocus6()
        {
            Screen screen = new Screen(0, 0);

            NotFocusableView n1 = new NotFocusableView("n1");
            NotFocusableView n2 = new NotFocusableView("n2");

            FocusableView f1 = new FocusableView("f1");
            FocusableView f2 = new FocusableView("f3");

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

            screen.AddView(rootView);
            screen.Start();

            Assert.IsTrue(f1.focused);
            Assert.AreEqual(screen.focusedView, f1);
        }

        [TestMethod]
        public void TestDefaultFocus7()
        {
            Screen screen = new Screen(0, 0);

            NotFocusableView n1 = new NotFocusableView("n1");
            NotFocusableView n2 = new NotFocusableView("n2");
            
            FocusableView f1 = new FocusableView("f1");
            FocusableView f2 = new FocusableView("f3");
            
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

            screen.AddView(rootView);
            screen.Start();

            Assert.IsTrue(f1.focused);
            Assert.AreEqual(screen.focusedView, f1);

            screen.HandleEvent(new KeyEvent().Init(new KeyEventArg(KeyCode.Down), KeyState.Pressed));

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

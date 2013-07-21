using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BomberEngine.Core;

namespace BomberEngineTests
{
    [TestClass]
    public class TimerManagerTests
    {
        private List<TimerCallback> callbacks = new List<TimerCallback>();

        [TestMethod]
        public void testSortingTimers1()
        {
            Reset();

            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallback1, 0.0f,  "timer1");
            manager.Schedule(TimerCallback1, 0.25f, "timer2");
            manager.Schedule(TimerCallback1, 0.25f, "timer3");
            manager.Schedule(TimerCallback1, 0.5f,  "timer4");
            manager.Schedule(TimerCallback1, 0.75f, "timer5");

            Timer timer = manager.RootTimer;
            Assert.AreEqual(timer.name, "timer1"); timer = timer.next;
            Assert.AreEqual(timer.name, "timer2"); timer = timer.next;
            Assert.AreEqual(timer.name, "timer3"); timer = timer.next;
            Assert.AreEqual(timer.name, "timer4"); timer = timer.next;
            Assert.AreEqual(timer.name, "timer5"); timer = timer.next;
            Assert.IsNull(timer);
        }

        [TestMethod]
        public void testSortingTimers2()
        {
            Reset();

            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallback1, 0.75f, "timer1");
            manager.Schedule(TimerCallback1, 0.5f,  "timer2");
            manager.Schedule(TimerCallback1, 0.25f, "timer3");
            manager.Schedule(TimerCallback1, 0.25f, "timer4");
            manager.Schedule(TimerCallback1, 0.0f,  "timer5");

            Timer timer = manager.RootTimer;
            Assert.AreEqual(timer.name, "timer5"); timer = timer.next;
            Assert.AreEqual(timer.name, "timer3"); timer = timer.next;
            Assert.AreEqual(timer.name, "timer4"); timer = timer.next;
            Assert.AreEqual(timer.name, "timer2"); timer = timer.next;
            Assert.AreEqual(timer.name, "timer1"); timer = timer.next;
            Assert.IsNull(timer);
        }

        [TestMethod]
        public void testAddTimer()
        {
            Reset();

            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallback1, 0.5f);

            manager.Update(0.25f);
            AssertCallbacks();
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback1);
            Assert.AreEqual(0, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback1);
            Assert.AreEqual(0, manager.Count());
        }

        [TestMethod]
        public void testAddTimers()
        {
            Reset();

            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallback1, 0.75f);
            manager.Schedule(TimerCallback2, 0.5f);

            manager.Update(0.25f);
            AssertCallbacks();
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2, TimerCallback1);
            Assert.AreEqual(0, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2, TimerCallback1);
            Assert.AreEqual(0, manager.Count());
        }

        [TestMethod]
        public void testAddMoreTimers()
        {
            Reset();

            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallback1, 0.75f);
            manager.Schedule(TimerCallback2, 0.5f);
            manager.Schedule(TimerCallback3, 0.5f);
            manager.Schedule(TimerCallback4, 1.0f);

            manager.Update(0.25f);
            AssertCallbacks();
            Assert.AreEqual(4, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2, TimerCallback3);
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2, TimerCallback3, TimerCallback1);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2, TimerCallback3, TimerCallback1, TimerCallback4);
            Assert.AreEqual(0, manager.Count());
        }

        [TestMethod]
        public void testAddMoreTimersLater()
        {
            Reset();

            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallback1, 0.75f);
            manager.Schedule(TimerCallback2, 0.5f);

            manager.Update(0.25f); // 0.25
            AssertCallbacks();
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f); // 0.5
            AssertCallbacks(TimerCallback2);
            Assert.AreEqual(1, manager.Count());

            manager.Schedule(TimerCallback3, 1.0f); // fires at 1.5
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f); // 0.75
            AssertCallbacks(TimerCallback2, TimerCallback1);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f); // 1.0
            AssertCallbacks(TimerCallback2, TimerCallback1);
            Assert.AreEqual(1, manager.Count());

            manager.Schedule(TimerCallback4, 0.15f); // fires at 1.15
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f); // 1.25
            AssertCallbacks(TimerCallback2, TimerCallback1, TimerCallback4);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f); // 1.5
            AssertCallbacks(TimerCallback2, TimerCallback1, TimerCallback4, TimerCallback3);
            Assert.AreEqual(0, manager.Count());

            manager.Update(0.25f); // 1.75
            AssertCallbacks(TimerCallback2, TimerCallback1, TimerCallback4, TimerCallback3);
            Assert.AreEqual(0, manager.Count());
        }

        [TestMethod]
        public void testCancelTimer()
        {
            Reset();

            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallback1, 0.75f);
            manager.Schedule(TimerCallback2, 0.5f);

            manager.Update(0.25f); // 0.25
            AssertCallbacks();
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f); // 0.5
            AssertCallbacks(TimerCallback2);
            Assert.AreEqual(1, manager.Count());

            manager.Schedule(TimerCallback3, 1.0f); // fires at 1.5
            Assert.AreEqual(2, manager.Count());

            manager.Cancel(TimerCallback1);

            manager.Update(0.25f); // 0.75
            AssertCallbacks(TimerCallback2);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f); // 1.0
            AssertCallbacks(TimerCallback2);
            Assert.AreEqual(1, manager.Count());

            manager.Schedule(TimerCallback4, 0.15f); // fires at 1.15
            Assert.AreEqual(2, manager.Count());

            manager.Cancel(TimerCallback3);

            manager.Update(0.25f); // 1.25
            AssertCallbacks(TimerCallback2, TimerCallback4);
            Assert.AreEqual(0, manager.Count());

            manager.Update(0.25f); // 1.5
            AssertCallbacks(TimerCallback2, TimerCallback4);
            Assert.AreEqual(0, manager.Count());

            manager.Update(0.25f); // 1.75
            AssertCallbacks(TimerCallback2, TimerCallback4);
            Assert.AreEqual(0, manager.Count());
        }

        private void TimerCallback1(Timer timer)
        {
            callbacks.Add(TimerCallback1);
        }

        private void TimerCallback2(Timer timer)
        {
            callbacks.Add(TimerCallback2);
        }

        private void TimerCallback3(Timer timer)
        {
            callbacks.Add(TimerCallback3);
        }

        private void TimerCallback4(Timer timer)
        {
            callbacks.Add(TimerCallback4);
        }

        private void AssertCallbacks(params TimerCallback[] expected)
        {
            TimerCallback[] actual = GetCallbacks();
            Assert.AreEqual(expected.Length, actual.Length);

            String message = "";
            for (int i = 0; i < expected.Length; ++i)
            {
                if (expected[i] != actual[i])
                {
                    message += expected[i] + "!=" + actual[i];
                }
            }

            Assert.IsTrue(message.Length == 0, message);
        }

        private TimerCallback[] GetCallbacks()
        {
            TimerCallback[] array = new TimerCallback[callbacks.Count];
            callbacks.CopyTo(array);
            return array;
        }

        private void Reset()
        {
            callbacks.Clear();
            Timer.freeRoot = null;
        }
    }

    class TestTimerManager : TimerManager
    {
        public Timer RootTimer
        {
            get { return rootCall; }
        }
    }
}

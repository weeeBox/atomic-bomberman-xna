using System;
using System.Collections.Generic;
using BomberEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BomberEngineTests
{
    using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

    [TestClass]
    public class TimerManagerTests
    {
        private List<TimerCallback1> callbacks1 = new List<TimerCallback1>();
        private List<TimerCallback2> callbacks2 = new List<TimerCallback2>();

        [TestMethod]
        public void testSortingTimers1()
        {
            Reset();

            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallbackEx1, 0.0f,  "timer1");
            manager.Schedule(TimerCallbackEx1, 0.25f, "timer2");
            manager.Schedule(TimerCallbackEx1, 0.25f, "timer3");
            manager.Schedule(TimerCallbackEx1, 0.5f,  "timer4");
            manager.Schedule(TimerCallbackEx1, 0.75f, "timer5");

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
            manager.Schedule(TimerCallbackEx1, 0.75f, "timer1");
            manager.Schedule(TimerCallbackEx1, 0.5f,  "timer2");
            manager.Schedule(TimerCallbackEx1, 0.25f, "timer3");
            manager.Schedule(TimerCallbackEx1, 0.25f, "timer4");
            manager.Schedule(TimerCallbackEx1, 0.0f,  "timer5");

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
        public void testAddTimerEx()
        {
            Reset();

            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallbackEx1, 0.5f);

            manager.Update(0.25f);
            AssertCallbacksEx();
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f);
            AssertCallbacksEx(TimerCallbackEx1);
            Assert.AreEqual(0, manager.Count());

            manager.Update(0.25f);
            AssertCallbacksEx(TimerCallbackEx1);
            Assert.AreEqual(0, manager.Count());
        }

        [TestMethod]
        public void testAddTimers()
        {
            Reset();

            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallbackEx1, 0.75f);
            manager.Schedule(TimerCallbackEx2, 0.5f);

            manager.Update(0.25f);
            AssertCallbacksEx();
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f);
            AssertCallbacksEx(TimerCallbackEx2);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f);
            AssertCallbacksEx(TimerCallbackEx2, TimerCallbackEx1);
            Assert.AreEqual(0, manager.Count());

            manager.Update(0.25f);
            AssertCallbacksEx(TimerCallbackEx2, TimerCallbackEx1);
            Assert.AreEqual(0, manager.Count());
        }

        [TestMethod]
        public void testAddTimersEx()
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
        public void testAddMoreTimersEx()
        {
            Reset();

            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallbackEx1, 0.75f);
            manager.Schedule(TimerCallbackEx2, 0.5f);
            manager.Schedule(TimerCallbackEx3, 0.5f);
            manager.Schedule(TimerCallbackEx4, 1.0f);

            manager.Update(0.25f);
            AssertCallbacksEx();
            Assert.AreEqual(4, manager.Count());

            manager.Update(0.25f);
            AssertCallbacksEx(TimerCallbackEx2, TimerCallbackEx3);
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f);
            AssertCallbacksEx(TimerCallbackEx2, TimerCallbackEx3, TimerCallbackEx1);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f);
            AssertCallbacksEx(TimerCallbackEx2, TimerCallbackEx3, TimerCallbackEx1, TimerCallbackEx4);
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
        public void testAddMoreTimersLaterEx()
        {
            Reset();

            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallbackEx1, 0.75f);
            manager.Schedule(TimerCallbackEx2, 0.5f);

            manager.Update(0.25f); // 0.25
            AssertCallbacksEx();
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f); // 0.5
            AssertCallbacksEx(TimerCallbackEx2);
            Assert.AreEqual(1, manager.Count());

            manager.Schedule(TimerCallbackEx3, 1.0f); // fires at 1.5
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f); // 0.75
            AssertCallbacksEx(TimerCallbackEx2, TimerCallbackEx1);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f); // 1.0
            AssertCallbacksEx(TimerCallbackEx2, TimerCallbackEx1);
            Assert.AreEqual(1, manager.Count());

            manager.Schedule(TimerCallbackEx4, 0.15f); // fires at 1.15
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f); // 1.25
            AssertCallbacksEx(TimerCallbackEx2, TimerCallbackEx1, TimerCallbackEx4);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f); // 1.5
            AssertCallbacksEx(TimerCallbackEx2, TimerCallbackEx1, TimerCallbackEx4, TimerCallbackEx3);
            Assert.AreEqual(0, manager.Count());

            manager.Update(0.25f); // 1.75
            AssertCallbacksEx(TimerCallbackEx2, TimerCallbackEx1, TimerCallbackEx4, TimerCallbackEx3);
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

        [TestMethod]
        public void testCancelTimerEx()
        {
            Reset();

            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallbackEx1, 0.75f);
            manager.Schedule(TimerCallbackEx2, 0.5f);

            manager.Update(0.25f); // 0.25
            AssertCallbacksEx();
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f); // 0.5
            AssertCallbacksEx(TimerCallbackEx2);
            Assert.AreEqual(1, manager.Count());

            manager.Schedule(TimerCallbackEx3, 1.0f); // fires at 1.5
            Assert.AreEqual(2, manager.Count());

            manager.Cancel(TimerCallbackEx1);

            manager.Update(0.25f); // 0.75
            AssertCallbacksEx(TimerCallbackEx2);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f); // 1.0
            AssertCallbacksEx(TimerCallbackEx2);
            Assert.AreEqual(1, manager.Count());

            manager.Schedule(TimerCallbackEx4, 0.15f); // fires at 1.15
            Assert.AreEqual(2, manager.Count());

            manager.Cancel(TimerCallbackEx3);

            manager.Update(0.25f); // 1.25
            AssertCallbacksEx(TimerCallbackEx2, TimerCallbackEx4);
            Assert.AreEqual(0, manager.Count());

            manager.Update(0.25f); // 1.5
            AssertCallbacksEx(TimerCallbackEx2, TimerCallbackEx4);
            Assert.AreEqual(0, manager.Count());

            manager.Update(0.25f); // 1.75
            AssertCallbacksEx(TimerCallbackEx2, TimerCallbackEx4);
            Assert.AreEqual(0, manager.Count());
        }

        private void TimerCallback1()
        {
            callbacks1.Add(TimerCallback1);
        }

        private void TimerCallback2()
        {
            callbacks1.Add(TimerCallback2);
        }

        private void TimerCallback3()
        {
            callbacks1.Add(TimerCallback3);
        }

        private void TimerCallback4()
        {
            callbacks1.Add(TimerCallback4);
        }

        private void TimerCallbackEx1(Timer timer)
        {
            callbacks2.Add(TimerCallbackEx1);
        }

        private void TimerCallbackEx2(Timer timer)
        {
            callbacks2.Add(TimerCallbackEx2);
        }

        private void TimerCallbackEx3(Timer timer)
        {
            callbacks2.Add(TimerCallbackEx3);
        }

        private void TimerCallbackEx4(Timer timer)
        {
            callbacks2.Add(TimerCallbackEx4);
        }

        private void AssertCallbacks(params TimerCallback1[] expected)
        {
            AssertCallbacksHelper(callbacks1, expected);
        }

        private void AssertCallbacksEx(params TimerCallback2[] expected)
        {
            AssertCallbacksHelper(callbacks2, expected);
        }

        private void AssertCallbacksHelper<T>(List<T> actual, params T[] expected) where T : class
        {   
            Assert.AreEqual(expected.Length, actual.Count);

            String message = "";
            for (int i = 0; i < expected.Length; ++i)
            {
                if (!expected[i].Equals(actual[i]))
                {
                    message += expected[i] + "!=" + actual[i];
                }
            }

            Assert.IsTrue(message.Length == 0, message);
        }

        private void Reset()
        {
            callbacks1.Clear();
            callbacks2.Clear();
            Timer.freeRoot = null;
        }
    }

    class TestTimerManager : TimerManager
    {
        public Timer RootTimer
        {
            get { return rootTimer; }
        }

        public Timer Schedule(TimerCallback2 callback, float delay, String name)
        {
            return Schedule(callback, delay, false, name);
        }
    }
}

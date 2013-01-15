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
        public void testAddTimer()
        {
            Reset();

            TimerManager manager = new TimerManager();
            manager.Schedule(TimerCallback1, 0.5f);

            manager.Update(0.25f);
            AssertCallbacks();
            Assert.AreEqual(1, manager.GetTimersCount());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback1);
            Assert.AreEqual(0, manager.GetTimersCount());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback1);
            Assert.AreEqual(0, manager.GetTimersCount());
        }

        [TestMethod]
        public void testAddTimers()
        {
            Reset();

            TimerManager manager = new TimerManager();
            manager.Schedule(TimerCallback1, 0.75f);
            manager.Schedule(TimerCallback2, 0.5f);

            manager.Update(0.25f);
            AssertCallbacks();
            Assert.AreEqual(2, manager.GetTimersCount());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2);
            Assert.AreEqual(1, manager.GetTimersCount());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2, TimerCallback1);
            Assert.AreEqual(0, manager.GetTimersCount());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2, TimerCallback1);
            Assert.AreEqual(0, manager.GetTimersCount());
        }

        [TestMethod]
        public void testAddMoreTimers()
        {
            Reset();

            TimerManager manager = new TimerManager();
            manager.Schedule(TimerCallback1, 0.75f);
            manager.Schedule(TimerCallback2, 0.5f);
            manager.Schedule(TimerCallback3, 0.5f);
            manager.Schedule(TimerCallback4, 1.0f);

            manager.Update(0.25f);
            AssertCallbacks();
            Assert.AreEqual(4, manager.GetTimersCount());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2, TimerCallback3);
            Assert.AreEqual(2, manager.GetTimersCount());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2, TimerCallback3, TimerCallback1);
            Assert.AreEqual(1, manager.GetTimersCount());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2, TimerCallback3, TimerCallback1, TimerCallback4);
            Assert.AreEqual(0, manager.GetTimersCount());
        }

        [TestMethod]
        public void testAddMoreTimersLater()
        {
            Reset();

            TimerManager manager = new TimerManager();
            manager.Schedule(TimerCallback1, 0.75f);
            manager.Schedule(TimerCallback2, 0.5f);

            manager.Update(0.25f); // 0.25
            AssertCallbacks();
            Assert.AreEqual(2, manager.GetTimersCount());

            manager.Update(0.25f); // 0.5
            AssertCallbacks(TimerCallback2);
            Assert.AreEqual(1, manager.GetTimersCount());

            manager.Schedule(TimerCallback3, 1.0f); // fires at 1.5
            Assert.AreEqual(2, manager.GetTimersCount());

            manager.Update(0.25f); // 0.75
            AssertCallbacks(TimerCallback2, TimerCallback1);
            Assert.AreEqual(1, manager.GetTimersCount());

            manager.Update(0.25f); // 1.0
            AssertCallbacks(TimerCallback2, TimerCallback1);
            Assert.AreEqual(1, manager.GetTimersCount());

            manager.Schedule(TimerCallback4, 0.15f); // fires at 1.15
            Assert.AreEqual(2, manager.GetTimersCount());

            manager.Update(0.25f); // 1.25
            AssertCallbacks(TimerCallback2, TimerCallback1, TimerCallback4);
            Assert.AreEqual(1, manager.GetTimersCount());

            manager.Update(0.25f); // 1.5
            AssertCallbacks(TimerCallback2, TimerCallback1, TimerCallback4, TimerCallback3);
            Assert.AreEqual(0, manager.GetTimersCount());

            manager.Update(0.25f); // 1.75
            AssertCallbacks(TimerCallback2, TimerCallback1, TimerCallback4, TimerCallback3);
            Assert.AreEqual(0, manager.GetTimersCount());
        }

        [TestMethod]
        public void testCancelTimer()
        {
            Reset();

            TimerManager manager = new TimerManager();
            Timer timer1 = manager.Schedule(TimerCallback1, 0.75f);
            manager.Schedule(TimerCallback2, 0.5f);

            manager.Update(0.25f); // 0.25
            AssertCallbacks();
            Assert.AreEqual(2, manager.GetTimersCount());

            manager.Update(0.25f); // 0.5
            AssertCallbacks(TimerCallback2);
            Assert.AreEqual(1, manager.GetTimersCount());

            Timer timer2 = manager.Schedule(TimerCallback3, 1.0f); // fires at 1.5
            Assert.AreEqual(2, manager.GetTimersCount());

            timer1.Cancel();

            manager.Update(0.25f); // 0.75
            AssertCallbacks(TimerCallback2);
            Assert.AreEqual(1, manager.GetTimersCount());

            manager.Update(0.25f); // 1.0
            AssertCallbacks(TimerCallback2);
            Assert.AreEqual(1, manager.GetTimersCount());

            manager.Schedule(TimerCallback4, 0.15f); // fires at 1.15
            Assert.AreEqual(2, manager.GetTimersCount());

            timer2.Cancel();

            manager.Update(0.25f); // 1.25
            AssertCallbacks(TimerCallback2, TimerCallback4);
            Assert.AreEqual(0, manager.GetTimersCount());

            manager.Update(0.25f); // 1.5
            AssertCallbacks(TimerCallback2, TimerCallback4);
            Assert.AreEqual(0, manager.GetTimersCount());

            manager.Update(0.25f); // 1.75
            AssertCallbacks(TimerCallback2, TimerCallback4);
            Assert.AreEqual(0, manager.GetTimersCount());
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
        }
    }
}

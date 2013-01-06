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
        private List<Timer> timers = new List<Timer>();

        [TestMethod]
        public void testAddTimer()
        {
            Reset();

            TimerManager manager = new TimerManager();
            manager.Schedule(TimerCallback1, 0.5f);

            manager.Update(0.25f);
            Assert.AreEqual(0, timers.Count);
            Assert.AreEqual(1, manager.TimersCount);
            Assert.AreEqual(1, manager.AliveTimersCount);

            manager.Update(0.25f);
            Assert.AreEqual(1, timers.Count);
            Assert.AreEqual(0, manager.TimersCount);
            Assert.AreEqual(0, manager.AliveTimersCount);
        }

        private void TimerCallback1(Timer timer)
        {
            timers.Add(timer);
        }

        private void TimerCallback2(Timer timer)
        {

        }

        private void TimerCallback3(Timer timer)
        {

        }

        private void TimerCallback4(Timer timer)
        {

        }

        private void Reset()
        {
            timers.Clear();
        }
    }
}

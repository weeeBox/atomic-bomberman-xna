using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BomberEngine.Util;
using BomberEngine.Core;

namespace BomberEngineTests
{
    [TestClass]
    public class ObjectsPoolTest
    {
        [TestMethod]
        public void TestNextObject()
        {
            TimerManager timerManager = new TimerManager();

            ObjectsPool<PoolDummy> pool = new ObjectsPool<PoolDummy>(timerManager);
            PoolDummy o1 = pool.NextObject();
            PoolDummy o2 = pool.NextObject();
            PoolDummy o3 = pool.NextObject();

            Assert.AreEqual(pool.size, 0);

            pool.RecycleObject(o1);
            pool.RecycleObject(o2);
            pool.RecycleObject(o3);
            Assert.AreEqual(pool.size, 3);

            PoolDummy o4 = pool.NextObject();
            PoolDummy o5 = pool.NextObject();
            PoolDummy o6 = pool.NextObject();
            Assert.AreEqual(pool.size, 0);

            Assert.AreEqual(o1, o4);
            Assert.AreEqual(o2, o5);
            Assert.AreEqual(o3, o6);
        }

        [TestMethod]
        public void TestNextObject2()
        {
            TimerManager timerManager = new TimerManager();

            ObjectsPool<PoolDummy> pool = new ObjectsPool<PoolDummy>(timerManager);
            PoolDummy o1 = pool.NextObject();
            PoolDummy o2 = pool.NextObject();
            PoolDummy o3 = pool.NextObject();

            Assert.AreEqual(pool.size, 0);

            pool.RecycleObject(o1);
            pool.RecycleObject(o2);
            Assert.AreEqual(pool.size, 2);

            PoolDummy o4 = pool.NextObject();
            PoolDummy o5 = pool.NextObject();
            PoolDummy o6 = pool.NextObject();
            Assert.AreEqual(pool.size, 0);

            Assert.AreEqual(o1, o4);
            Assert.AreEqual(o2, o5);
            Assert.AreNotEqual(o3, o6);
        }
    }

    class PoolDummy : ObjectsPoolEntry<PoolDummy>
    {
        private static int s_nextId;
        private int m_id;

        public PoolDummy()
        {
            m_id = ++s_nextId;
        }

        public int id
        {
            get { return m_id; }
        }
    }
}

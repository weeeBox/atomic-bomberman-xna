using BomberEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BomberEngineTests
{
    [TestClass]
    public class ObjectsPoolTest
    {
        [TestMethod]
        public void TestNextObject()
        {
            TimerManager timerManager = new TimerManager();

            ObjectsPool<PoolDummy> pool = new ObjectsPool<PoolDummy>();
            PoolDummy o1 = pool.NextObject();
            PoolDummy o2 = pool.NextObject();
            PoolDummy o3 = pool.NextObject();

            Assert.AreEqual(pool.size, 0);

            o1.Recycle();
            o2.Recycle();
            o3.Recycle();
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

            ObjectsPool<PoolDummy> pool = new ObjectsPool<PoolDummy>();
            PoolDummy o1 = pool.NextObject();
            PoolDummy o2 = pool.NextObject();
            PoolDummy o3 = pool.NextObject();

            Assert.AreEqual(pool.size, 0);

            o1.Recycle();
            o2.Recycle();
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

    class PoolDummy : ObjectsPoolEntry
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

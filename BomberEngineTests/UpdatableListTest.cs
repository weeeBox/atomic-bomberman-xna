using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BomberEngine.Core;

namespace BomberEngineTests
{
    [TestClass]
    public class UpdatableListTest
    {
        [TestMethod]
        public void TestAdding()
        {
            UpdatableList list = new UpdatableList();
            list.Add(new Updatable1(list));
            list.Add(new Updatable1(list));
            list.Add(new Updatable1(list));
            list.Add(new Updatable1(list));

            Assert.IsTrue(list.Count() == 4);
        }

        [TestMethod]
        public void TestRemoveItemsOnUpdate()
        {
            UpdatableList list = new UpdatableList();
            list.Add(new Updatable1(list));
            list.Add(new Updatable1(list));
            list.Add(new Updatable1(list));
            list.Add(new Updatable1(list));

            list.Update(0.016f);

            Assert.AreEqual(list.Count(), 0);
        }

        [TestMethod]
        public void TestRemoveSomeItemsOnUpdate()
        {
            UpdatableList list = new UpdatableList();
            list.Add(new Updatable2(list));
            list.Add(new Updatable1(list));
            list.Add(new Updatable1(list));
            list.Add(new Updatable1(list));

            list.Update(0.016f);

            Assert.AreEqual(list.Count(), 1);
        }

        [TestMethod]
        public void TestRemoveSomeItemsOnUpdate2()
        {
            UpdatableList list = new UpdatableList();
            list.Add(new Updatable2(list));
            list.Add(new Updatable2(list));
            list.Add(new Updatable1(list));
            list.Add(new Updatable1(list));

            list.Update(0.016f);

            Assert.AreEqual(list.Count(), 2);
        }

        [TestMethod]
        public void TestRemoveSomeItemsOnUpdate3()
        {
            UpdatableList list = new UpdatableList();
            list.Add(new Updatable2(list));
            list.Add(new Updatable2(list));
            list.Add(new Updatable2(list));
            list.Add(new Updatable1(list));

            list.Update(0.016f);

            Assert.AreEqual(list.Count(), 3);
        }

        [TestMethod]
        public void TestRemoveSomeItemsOnUpdate4()
        {
            UpdatableList list = new UpdatableList();
            list.Add(new Updatable2(list));
            list.Add(new Updatable2(list));
            list.Add(new Updatable2(list));
            list.Add(new Updatable2(list));

            list.Update(0.016f);

            Assert.AreEqual(list.Count(), 4);
        }

        [TestMethod]
        public void TestRemoveSomeItemsOnUpdate5()
        {
            UpdatableList list = new UpdatableList();
            list.Add(new Updatable1(list));
            list.Add(new Updatable2(list));
            list.Add(new Updatable2(list));
            list.Add(new Updatable2(list));

            list.Update(0.016f);

            Assert.AreEqual(list.Count(), 3);
        }

        [TestMethod]
        public void TestRemoveSomeItemsOnUpdate6()
        {
            UpdatableList list = new UpdatableList();
            list.Add(new Updatable1(list));
            list.Add(new Updatable1(list));
            list.Add(new Updatable2(list));
            list.Add(new Updatable2(list));

            list.Update(0.016f);

            Assert.AreEqual(list.Count(), 2);
        }

        [TestMethod]
        public void TestRemoveSomeItemsOnUpdate7()
        {
            UpdatableList list = new UpdatableList();
            list.Add(new Updatable1(list));
            list.Add(new Updatable1(list));
            list.Add(new Updatable1(list));
            list.Add(new Updatable2(list));

            list.Update(0.016f);

            Assert.AreEqual(list.Count(), 1);
        }

        [TestMethod]
        public void TestRemoveSomeItemsOnUpdate8()
        {
            UpdatableList list = new UpdatableList();
            list.Add(new Updatable1(list));
            list.Add(new Updatable2(list));
            list.Add(new Updatable1(list));
            list.Add(new Updatable2(list));

            list.Update(0.016f);

            Assert.AreEqual(list.Count(), 2);
        }

        [TestMethod]
        public void TestRemoveSomeItemsOnUpdate9()
        {
            UpdatableList list = new UpdatableList();
            list.Add(new Updatable2(list));
            list.Add(new Updatable1(list));
            list.Add(new Updatable2(list));
            list.Add(new Updatable1(list));

            list.Update(0.016f);

            Assert.AreEqual(list.Count(), 2);
        }

        [TestMethod]
        public void TestRemoveSomeItemsOnUpdate10()
        {
            UpdatableList list = new UpdatableList();
            list.Add(new Updatable2(list));
            list.Add(new Updatable1(list));
            list.Add(new Updatable1(list));
            list.Add(new Updatable2(list));

            list.Update(0.016f);

            Assert.AreEqual(list.Count(), 2);
        }

        [TestMethod]
        public void TestRemoveSomeItemsOnUpdate11()
        {
            UpdatableList list = new UpdatableList();
            list.Add(new Updatable1(list));
            list.Add(new Updatable2(list));
            list.Add(new Updatable2(list));
            list.Add(new Updatable1(list));

            list.Update(0.016f);

            Assert.AreEqual(list.Count(), 2);
        }
    }

    class Updatable1 : Updatable
    {
        private UpdatableList list;

        public Updatable1(UpdatableList list)
        {   
            this.list = list;
        }

        public void Update(float delta)
        {
            list.Remove(this);
        }
    }

    class Updatable2 : Updatable
    {
        private UpdatableList list;

        public Updatable2(UpdatableList list)
        {
            this.list = list;
        }

        public void Update(float delta)
        {
            
        }
    }
}

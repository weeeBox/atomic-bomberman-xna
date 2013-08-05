using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BomberEngine.Core.Storage;
using BomberEngine.Core;
using System.IO;

namespace BomberEngineTests
{
    [TestClass]
    public class SharedStorageTest
    {
        [TestMethod]
        public void TestSaveLoad()
        {
            TimerManager timerManager = new TimerManager();

            SharedStorage storage = CreateStorage("storage", timerManager);
            storage.Set("int", 10);
            storage.Set("float", 3.14f);
            storage.Set("bool", true);
            storage.Set("string", "This is a string");

            timerManager.Update(0.016f);
            storage = CreateStorage("storage", timerManager, false);
            Assert.AreEqual(10, storage.GetInt("int"));
            Assert.AreEqual(3.14f, storage.GetFloat("float"));
            Assert.AreEqual(true, storage.GetBool("bool"));
            Assert.AreEqual("This is a string", storage.GetString("string"));
        }

        [TestMethod]
        public void TestSaveLoad2()
        {
            TimerManager timerManager = new TimerManager();

            SharedStorage storage = CreateStorage("storage", timerManager);
            Assert.AreEqual(0, storage.GetInt("int"));
            Assert.AreEqual(0.0f, storage.GetFloat("float"));
            Assert.AreEqual(false, storage.GetBool("bool"));
            Assert.AreEqual(null, storage.GetString("string"));
        }

        [TestMethod]
        public void TestSaveLoad3()
        {
            TimerManager timerManager = new TimerManager();

            SharedStorage storage = CreateStorage("storage", timerManager);
            Assert.AreEqual(10, storage.GetInt("int", 10));
            Assert.AreEqual(3.14f, storage.GetFloat("float", 3.14f));
            Assert.AreEqual(true, storage.GetBool("bool", true));
            Assert.AreEqual("default", storage.GetString("string", "default"));
        }

        [TestMethod]
        public void TestSaveLoad4()
        {
            TimerManager timerManager = new TimerManager();

            SharedStorage storage = CreateStorage("storage", timerManager);
            storage.Set("int", 10);

            timerManager.Update(0.016f);
            storage = CreateStorage("storage", timerManager, false);
            Assert.AreEqual(10, storage.GetFloat("int"));
        }

        [TestMethod]
        public void TestSaveLoad5()
        {
            TimerManager timerManager = new TimerManager();

            SharedStorage storage = CreateStorage("storage", timerManager);
            storage.Set("int", 10);
            storage.Set("float", 3.14f);
            storage.Set("bool", true);
            storage.Set("string", "This is a string");

            Assert.AreEqual(1, timerManager.Count());
            storage.Destroy();
            Assert.AreEqual(0, timerManager.Count());

            storage = CreateStorage("storage", timerManager, false);
            Assert.AreEqual(10, storage.GetInt("int"));
            Assert.AreEqual(3.14f, storage.GetFloat("float"));
            Assert.AreEqual(true, storage.GetBool("bool"));
            Assert.AreEqual("This is a string", storage.GetString("string"));
        }

        [TestMethod]
        public void TestClear()
        {
            TimerManager timerManager = new TimerManager();

            SharedStorage storage = CreateStorage("storage", timerManager);
            storage.Set("int", 10);
            storage.Set("float", 3.14f);
            storage.Set("bool", true);
            storage.Set("string", "This is a string");

            timerManager.Update(0.016f);

            storage = CreateStorage("storage", timerManager, false);
            storage.Clear();

            timerManager.Update(0.016f);

            storage = CreateStorage("storage", timerManager, false);
            Assert.AreEqual(0, storage.GetInt("int"));
            Assert.AreEqual(0.0f, storage.GetFloat("float"));
            Assert.AreEqual(false, storage.GetBool("bool"));
            Assert.AreEqual(null, storage.GetString("string"));
        }

        [TestMethod]
        public void TestRemove()
        {
            TimerManager timerManager = new TimerManager();

            SharedStorage storage = CreateStorage("storage", timerManager);
            storage.Set("int", 10);
            storage.Set("float", 3.14f);
            storage.Set("bool", true);
            storage.Set("string", "This is a string");
            storage.Destroy();

            storage = CreateStorage("storage", timerManager, false);

            Assert.AreEqual(0, timerManager.Count());

            storage.Remove("int");
            Assert.AreEqual(1, timerManager.Count());

            storage.Remove("int");
            Assert.AreEqual(1, timerManager.Count());
            
            storage.Remove("float");
            Assert.AreEqual(1, timerManager.Count());

            timerManager.Update(0.016f);

            storage = CreateStorage("storage", timerManager, false);
            Assert.AreEqual(0, storage.GetInt("int"));
            Assert.AreEqual(0.0f, storage.GetFloat("float"));
            Assert.AreEqual(true, storage.GetBool("bool"));
            Assert.AreEqual("This is a string", storage.GetString("string"));
        }

        [TestMethod]
        public void TestRemove2()
        {
            TimerManager timerManager = new TimerManager();

            SharedStorage storage = CreateStorage("storage", timerManager);
            storage.Set("int", 10);
            storage.Set("float", 3.14f);
            storage.Set("bool", true);
            storage.Set("string", "This is a string");
            storage.Destroy();

            storage = CreateStorage("storage", timerManager, false);

            Assert.AreEqual(0, timerManager.Count());

            storage.Remove("foo");
            Assert.AreEqual(0, timerManager.Count());

            timerManager.Update(0.016f);

            storage = CreateStorage("storage", timerManager, false);
            Assert.AreEqual(10, storage.GetInt("int"));
            Assert.AreEqual(3.14f, storage.GetFloat("float"));
            Assert.AreEqual(true, storage.GetBool("bool"));
            Assert.AreEqual("This is a string", storage.GetString("string"));
        }

        private SharedStorage CreateStorage(String filename, TimerManager manager, bool clear = true)
        {
            if (clear && File.Exists(filename))
                File.Delete(filename);

            return new SharedStorage(filename, manager);
        }
    }
}
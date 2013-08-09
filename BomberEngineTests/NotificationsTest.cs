using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BomberEngine.Core.Events;
using BomberEngine.Core;

namespace BomberEngineTests
{
    [TestClass]
    public class NotificationsTest
    {
        private NotificationCenter notifications;

        [TestMethod]
        public void TestPostImmediately0()
        {
            List<String> result = new List<String>();

            notifications = new NotificationCenter(new TimerManager());
            notifications.PostImmediately(this, "name", result);

            Check(result);
        }

        [TestMethod]
        public void TestPostImmediately1()
        {
            List<String> result = new List<String>();

            notifications = new NotificationCenter(new TimerManager());
            notifications.Register("name", Callback1);
            notifications.Register("name", Callback2);
            notifications.Register("name", Callback3);

            notifications.PostImmediately(this, "name", result);

            Check(result, "Callback1", "Callback2", "Callback3");
        }

        [TestMethod]
        public void TestPostImmediately2()
        {
            List<String> result = new List<String>();

            notifications = new NotificationCenter(new TimerManager());
            notifications.Register("name1", Callback1);
            notifications.Register("name1", Callback2);
            notifications.Register("name1", Callback3);

            notifications.Register("name2", Callback3);
            notifications.Register("name2", Callback2);
            notifications.Register("name2", Callback1);

            notifications.PostImmediately(this, "name2", result);

            Check(result, "Callback3", "Callback2", "Callback1");
        }

        [TestMethod]
        public void TestPostImmediately3()
        {
            List<String> result = new List<String>();

            notifications = new NotificationCenter(new TimerManager());
            notifications.Register("name1", Callback1);
            notifications.Register("name1", Callback2);
            notifications.Register("name1", Callback3);

            notifications.PostImmediately(this, "name2", result);

            Check(result);
        }

        [TestMethod]
        public void TestPostImmediately4()
        {
            List<String> result = new List<String>();

            notifications = new NotificationCenter(new TimerManager());
            notifications.Register("name", Callback1);
            notifications.Register("name", Callback2);
            notifications.Register("name", Callback3);

            notifications.UnregisterAll(Callback1);
            
            notifications.PostImmediately(this, "name", result);

            Check(result, "Callback2", "Callback3");
        }

        [TestMethod]
        public void TestPostImmediately5()
        {
            List<String> result = new List<String>();

            notifications = new NotificationCenter(new TimerManager());
            notifications.Register("name", Callback1);
            notifications.Register("name", Callback2);
            notifications.Register("name", Callback3);

            notifications.UnregisterAll(this);

            notifications.PostImmediately(this, "name", result);

            Check(result);
        }

        [TestMethod]
        public void TestPostImmediately6()
        {
            List<String> result = new List<String>();

            Dummy dummy = new Dummy();

            notifications = new NotificationCenter(new TimerManager());
            notifications.Register("name", Callback1);
            notifications.Register("name", dummy.Callback1);
            notifications.Register("name", Callback2);
            notifications.Register("name", dummy.Callback2);
            notifications.Register("name", Callback3);
            notifications.Register("name", dummy.Callback3);

            notifications.PostImmediately(this, "name", result);

            Check(result, "Callback1", "Dummy1", "Callback2", "Dummy2", "Callback3", "Dummy3");
            notifications.UnregisterAll(this);

            notifications.PostImmediately(this, "name", result);
            Check(result, "Dummy1", "Dummy2", "Dummy3");
        }

        [TestMethod]
        public void TestPostImmediately7()
        {
            List<String> result = new List<String>();

            notifications = new NotificationCenter(new TimerManager());
            notifications.Register("name", Callback4);
            notifications.Register("name", Callback1);
            notifications.Register("name", Callback5);
            notifications.Register("name", Callback3);
            
            notifications.PostImmediately(this, "name", result);

            Check(result, "Callback4", "Callback5");
        }

        [TestMethod]
        public void TestPost0()
        {
            TimerManager timerManager = new TimerManager();

            List<String> result = new List<String>();

            notifications = new NotificationCenter(timerManager);
            notifications.Register("name", Callback1);
            notifications.Register("name", Callback2);
            notifications.Register("name", Callback3);

            notifications.Post(this, "name", result);

            Check(result);
            Assert.AreEqual(timerManager.Count(), 1);

            timerManager.Update(0.016f);

            Check(result, "Callback1", "Callback2", "Callback3");
            Assert.AreEqual(timerManager.Count(), 0);
        }

        [TestMethod]
        public void TestPost1()
        {
            TimerManager timerManager = new TimerManager();

            List<String> result = new List<String>();

            notifications = new NotificationCenter(timerManager);
            notifications.Register("name1", Callback1);
            notifications.Register("name2", Callback2);
            notifications.Register("name3", Callback3);

            notifications.Post(this, "name1", result);
            notifications.Post(this, "name2", result);
            notifications.Post(this, "name3", result);

            Check(result);
            Assert.AreEqual(timerManager.Count(), 3);

            timerManager.Update(0.016f);

            Check(result, "Callback1", "Callback2", "Callback3");
            Assert.AreEqual(timerManager.Count(), 0);
        }

        [TestMethod]
        public void TestPost2()
        {
            TimerManager timerManager = new TimerManager();

            List<String> result = new List<String>();

            notifications = new NotificationCenter(timerManager);
            notifications.Register("name1", Callback1);
            notifications.Register("name2", Callback2);
            notifications.Register("name3", Callback3);

            notifications.Post(this, "name1", result);
            notifications.Post(this, "name1", result);
            notifications.Post(this, "name1", result);

            Check(result);
            Assert.AreEqual(timerManager.Count(), 3);

            timerManager.Update(0.016f);

            Check(result, "Callback1", "Callback1", "Callback1");
            Assert.AreEqual(timerManager.Count(), 0);
        }

        [TestMethod]
        public void TestPost3()
        {
            TimerManager timerManager = new TimerManager();

            List<String> result = new List<String>();

            notifications = new NotificationCenter(timerManager);
            notifications.Register("name1", Callback6);
            notifications.Register("name2", Callback7);

            notifications.Post(this, "name1", result);
            
            Check(result);
            Assert.AreEqual(timerManager.Count(), 1);

            timerManager.Update(0.016f);
            Assert.AreEqual(timerManager.Count(), 1);

            Check(result, "Callback6");
            timerManager.Update(0.016f);

            Check(result, "Callback7");
            Assert.AreEqual(timerManager.Count(), 0);
        }

        [TestMethod]
        public void TestDestroy()
        {
            TimerManager timerManager = new TimerManager();

            List<String> result = new List<String>();

            notifications = new NotificationCenter(timerManager);
            notifications.Register("name1", Callback1);
            notifications.Register("name2", Callback2);
            notifications.Register("name3", Callback3);

            notifications.Post(this, "name1", result);
            notifications.Post(this, "name2", result);
            notifications.Post(this, "name3", result);

            Check(result);
            Assert.AreEqual(timerManager.Count(), 3);

            notifications.Destroy();
            Assert.AreEqual(timerManager.Count(), 0);

            timerManager.Update(0.016f);
            Check(result);
        }

        private void Callback1(Notification notification)
        {
            List<String> result = notification.data as List<String>;
            result.Add("Callback1");
        }

        private void Callback2(Notification notification)
        {
            List<String> result = notification.data as List<String>;
            result.Add("Callback2");
        }

        private void Callback3(Notification notification)
        {
            List<String> result = notification.data as List<String>;
            result.Add("Callback3");
        }

        private void Callback4(Notification notification)
        {
            List<String> result = notification.data as List<String>;
            result.Add("Callback4");

            notifications.UnregisterAll(Callback1);
        }

        private void Callback5(Notification notification)
        {
            List<String> result = notification.data as List<String>;
            result.Add("Callback5");

            notifications.UnregisterAll(Callback3);
        }

        private void Callback6(Notification notification)
        {
            List<String> result = notification.data as List<String>;
            result.Add("Callback6");

            notifications.Post(this, "name2", result);
        }

        private void Callback7(Notification notification)
        {
            List<String> result = notification.data as List<String>;
            result.Add("Callback7");
        }

        private void Check(List<String> result, params String[] values)
        {
            Assert.AreEqual(result.Count, values.Length);
            for (int i = 0; i < values.Length; ++i)
            {
                Assert.AreEqual(result[i], values[i]);
            }

            result.Clear();
        }
    }

    class Dummy
    {
        public void Callback1(Notification notification)
        {
            List<String> result = notification.data as List<String>;
            result.Add("Dummy1");
        }

        public void Callback2(Notification notification)
        {
            List<String> result = notification.data as List<String>;
            result.Add("Dummy2");
        }

        public void Callback3(Notification notification)
        {
            List<String> result = notification.data as List<String>;
            result.Add("Dummy3");
        }
    }

    // descriptive names is my everything...
}

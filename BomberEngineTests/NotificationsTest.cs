using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BomberEngine.Core.Events;

namespace BomberEngineTests
{
    [TestClass]
    public class NotificationsTest
    {
        private Notifications notifications;

        [TestMethod]
        public void TestPostImmediately0()
        {
            List<String> result = new List<String>();

            notifications = new Notifications();
            notifications.PostImmediately("name", result);

            Check(result);
        }

        [TestMethod]
        public void TestPostImmediately1()
        {
            List<String> result = new List<String>();

            notifications = new Notifications();
            notifications.Register("name", Callback1);
            notifications.Register("name", Callback2);
            notifications.Register("name", Callback3);

            notifications.PostImmediately("name", result);

            Check(result, "Callback1", "Callback2", "Callback3");
        }

        [TestMethod]
        public void TestPostImmediately2()
        {
            List<String> result = new List<String>();

            notifications = new Notifications();
            notifications.Register("name1", Callback1);
            notifications.Register("name1", Callback2);
            notifications.Register("name1", Callback3);

            notifications.Register("name2", Callback3);
            notifications.Register("name2", Callback2);
            notifications.Register("name2", Callback1);

            notifications.PostImmediately("name2", result);

            Check(result, "Callback3", "Callback2", "Callback1");
        }

        [TestMethod]
        public void TestPostImmediately3()
        {
            List<String> result = new List<String>();

            notifications = new Notifications();
            notifications.Register("name1", Callback1);
            notifications.Register("name1", Callback2);
            notifications.Register("name1", Callback3);

            notifications.PostImmediately("name2", result);

            Check(result);
        }

        [TestMethod]
        public void TestPostImmediately4()
        {
            List<String> result = new List<String>();

            notifications = new Notifications();
            notifications.Register("name", Callback1);
            notifications.Register("name", Callback2);
            notifications.Register("name", Callback3);

            notifications.Remove(Callback1);
            
            notifications.PostImmediately("name", result);

            Check(result, "Callback2", "Callback3");
        }

        [TestMethod]
        public void TestPostImmediately5()
        {
            List<String> result = new List<String>();

            notifications = new Notifications();
            notifications.Register("name", Callback1);
            notifications.Register("name", Callback2);
            notifications.Register("name", Callback3);

            notifications.RemoveAll(this);

            notifications.PostImmediately("name", result);

            Check(result);
        }

        [TestMethod]
        public void TestPostImmediately6()
        {
            List<String> result = new List<String>();

            Dummy dummy = new Dummy();

            notifications = new Notifications();
            notifications.Register("name", Callback1);
            notifications.Register("name", dummy.Callback1);
            notifications.Register("name", Callback2);
            notifications.Register("name", dummy.Callback2);
            notifications.Register("name", Callback3);
            notifications.Register("name", dummy.Callback3);

            notifications.PostImmediately("name", result);

            Check(result, "Callback1", "Dummy1", "Callback2", "Dummy2", "Callback3", "Dummy3");
            notifications.RemoveAll(this);

            result.Clear();
            notifications.PostImmediately("name", result);

            Check(result, "Dummy1", "Dummy2", "Dummy3");
        }

        [TestMethod]
        public void TestPostImmediately7()
        {
            List<String> result = new List<String>();

            notifications = new Notifications();
            notifications.Register("name", Callback4);
            notifications.Register("name", Callback1);
            notifications.Register("name", Callback5);
            notifications.Register("name", Callback3);
            
            result.Clear();
            notifications.PostImmediately("name", result);

            Check(result, "Callback4", "Callback5");
        }

        [TestMethod]
        public void TestPost0()
        {
            List<String> result = new List<String>();

            notifications = new Notifications();
            notifications.Register("name", Callback1);
            notifications.Register("name", Callback2);
            notifications.Register("name", Callback3);

            result.Clear();
            notifications.Post("name", result);

            Check(result, "Callback1", "Callback2", "Callback3");
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

            notifications.Remove(Callback1);
        }

        private void Callback5(Notification notification)
        {
            List<String> result = notification.data as List<String>;
            result.Add("Callback5");

            notifications.Remove(Callback3);
        }

        private void Check(List<String> result, params String[] values)
        {
            Assert.AreEqual(result.Count, values.Length);
            for (int i = 0; i < values.Length; ++i)
            {
                Assert.AreEqual(result[i], values[i]);
            }
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

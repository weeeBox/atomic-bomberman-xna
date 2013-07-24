using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;
using BomberEngine.Debugging;

namespace BomberEngine.Core.Events
{
    public delegate void NotificationDelegate(Notification notification);

    public class Notifications
    {
        private IDictionary<String, NotificationDelegateList> m_registerMap;
        private FastLinkedList<Notification> m_notificationQueue;

        public Notifications()
        {
            m_registerMap = new Dictionary<String, NotificationDelegateList>();
        }

        public void Register(String name, NotificationDelegate del)
        {
            Debug.Assert(name != null);
            Debug.Assert(del != null);

            NotificationDelegateList list = FindList(name);
            if (list == null)
            {
                list = new NotificationDelegateList();
                m_registerMap[name] = list;
            }

            list.Add(del);
        }

        public bool Remove(String name, NotificationDelegate del)
        {
            NotificationDelegateList list = FindList(name);
            if (list != null)
            {
                return list.Remove(del);
            }

            return false;
        }

        public bool Remove(NotificationDelegate del)
        {
            bool removed = false;
            foreach (KeyValuePair<String, NotificationDelegateList> e in m_registerMap)
            {   
                NotificationDelegateList list = e.Value;
                removed |= list.Remove(del);
            }
            return removed;
        }

        public bool RemoveAll(Object target)
        {
            bool removed = false;
            foreach (KeyValuePair<String, NotificationDelegateList> e in m_registerMap)
            {
                NotificationDelegateList list = e.Value;
                removed |= list.RemoveAll(target);
            }
            return removed;
        }

        public void Post(Notification notification)
        {
            if (m_notificationQueue == null)
            {
                m_notificationQueue = new FastLinkedList<Notification>();
            }
            m_notificationQueue.AddLastItem(notification);
        }

        public void Cancel(Notification notification)
        {
            if (m_notificationQueue != null)
            {
                m_notificationQueue.RemoveItem(notification);
            }
        }

        public void PostImmediately(Notification notification)
        {
            String name = notification.name;
            NotificationDelegateList list = FindList(name);
            if (list != null)
            {
                list.NotifyDelegates(notification);
            }
        }

        private NotificationDelegateList FindList(String name)
        {
            NotificationDelegateList list;
            if (m_registerMap.TryGetValue(name, out list))
            {
                return list;
            }

            return null;
        }
    }

    public class Notification : ObjectPoolEntry<Notification>
    {
        private String m_name;
        private Object m_data;

        internal void Init(String name)
        {
            m_name = name;
        }

        protected override void OnRecycleObject()
        {
            m_name = null;
            m_data = null;
        }

        public String name
        {
            get { return m_name; }
        }

        public Object data
        {
            get { return m_data; }
            set { m_data = value; }
        }
    }

    internal class NotificationDelegateList : BaseList<NotificationDelegate>
    {
        public NotificationDelegateList()
            : base(NullNotificationDelegate)
        {
        }

        public override bool Add(NotificationDelegate del)
        {
            Debug.Assert(!Contains(del));
            return base.Add(del);
        }

        public bool RemoveAll(Object target)
        {
            bool removed = false;
            for (int i = 0; i < list.Count; ++i)
            {
                NotificationDelegate del = list[i];
                if (del.Target == target)
                {
                    RemoveAt(i); // it's safe: the list size will be changed on the next update
                    removed = true;
                }
            }

            return removed;
        }

        public void NotifyDelegates(Notification notification)
        {
            int delegatesCount = list.Count;
            for (int i = 0; i < delegatesCount; ++i)
            {
                NotificationDelegate del = list[i];
                del(notification);
            }
            ClearRemoved();
        }

        private static void NullNotificationDelegate(Notification notification)
        {
        }
    }
}

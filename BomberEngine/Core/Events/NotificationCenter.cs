using System;
using System.Collections.Generic;

namespace BomberEngine
{
    public delegate void NotificationDelegate(Notification notification);

    public class NotificationCenter : IDestroyable
    {
        private TimerManager m_timerManager;

        private IDictionary<String, NotificationDelegateList> m_registerMap;
        private ObjectsPool<Notification> m_notificatoinsPool;

        public NotificationCenter()
            : this(Application.TimerManager())
        {
        }

        public NotificationCenter(TimerManager timerManager)
        {
            m_timerManager = timerManager;
            m_registerMap = new Dictionary<String, NotificationDelegateList>();
            m_notificatoinsPool = new ObjectsPool<Notification>();
        }

        public void Destroy()
        {
            CancelScheduledPosts();
        }

        public void Register(String name, NotificationDelegate del)
        {
            Assert.True(name != null);
            Assert.True(del != null);

            NotificationDelegateList list = FindList(name);
            if (list == null)
            {
                list = new NotificationDelegateList();
                m_registerMap[name] = list;
            }

            list.Add(del);
        }

        public bool Unregister(String name, NotificationDelegate del)
        {
            NotificationDelegateList list = FindList(name);
            if (list != null)
            {
                return list.Remove(del);
            }

            return false;
        }

        public bool UnregisterAll(NotificationDelegate del)
        {
            bool removed = false;
            foreach (KeyValuePair<String, NotificationDelegateList> e in m_registerMap)
            {   
                NotificationDelegateList list = e.Value;
                removed |= list.Remove(del);
            }
            return removed;
        }

        public bool UnregisterAll(Object target)
        {
            bool removed = false;
            foreach (KeyValuePair<String, NotificationDelegateList> e in m_registerMap)
            {
                NotificationDelegateList list = e.Value;
                removed |= list.RemoveAll(target);
            }
            return removed;
        }

        public void Post(Object sender, String name, Object data = null, Object data2 = null, Object data3 = null, Object data4 = null)
        {
            NotificationDelegateList list = FindList(name);
            if (list != null && list.Count() > 0)
            {
                Notification notification = m_notificatoinsPool.NextObject();
                notification.Init(sender, name, data, data2, data3, data4);

                SchedulePost(notification);
            }
        }

        public void PostImmediately(Object sender, String name, Object data = null, Object data2 = null, Object data3 = null, Object data4 = null)
        {   
            NotificationDelegateList list = FindList(name);
            if (list != null && list.Count() > 0)
            {
                Notification notification = m_notificatoinsPool.NextObject();
                notification.Init(sender, name, data, data2, data3, data4);

                list.NotifyDelegates(notification);
                notification.Recycle();
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
            notification.Recycle();
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

        private void SchedulePost(Notification notification)
        {
            Timer timer = m_timerManager.Schedule(PostCallback);
            timer.userData = notification;
        }

        private void CancelScheduledPosts()
        {
            m_timerManager.Cancel(PostCallback);
        }

        private void PostCallback(Timer timer)
        {
            Notification notification = timer.userData as Notification;
            Assert.True(notification != null);

            PostImmediately(notification);
        }
    }

    public class Notification : ObjectsPoolEntry
    {
        private Object m_sender;
        private String m_name;

        private Object m_data;
        private Object m_data2;
        private Object m_data3;
        private Object m_data4;

        internal void Init(Object sender, String name, Object data = null, Object data2 = null, Object data3 = null, Object data4 = null)
        {
            m_sender = sender;
            m_name = name;

            m_data = data;
            m_data2 = data2;
            m_data3 = data3;
            m_data4 = data4;
        }

        protected override void OnRecycleObject()
        {
            m_sender = null;
            m_name = null;

            m_data = null;
            m_data2 = null;
            m_data3 = null;
            m_data4 = null;
        }

        public String name
        {
            get { return m_name; }
        }

        public T GetData<T>()
        {   
            return (T)m_data;
        }

        public T GetNotNullData<T>()
        {
            T t = (T)m_data;
            Debug.AssertNotNull(t);
            return t;
        }

        public T GetData2<T>()
        {
            return (T)m_data2;
        }

        public T GetNotNullData2<T>()
        {
            T t = (T)m_data2;
            Debug.AssertNotNull(t);
            return t;
        }

        public T GetData3<T>()
        {
            return (T)m_data3;
        }

        public T GetNotNullData3<T>()
        {
            T t = (T)m_data3;
            Debug.AssertNotNull(t);
            return t;
        }

        public T GetData4<T>()
        {
            return (T)m_data4;
        }

        public T GetNotNullData4<T>()
        {
            T t = (T)m_data4;
            Debug.AssertNotNull(t);
            return t;
        }

        public Object data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        public Object data2
        {
            get { return m_data2; }
            set { m_data2 = value; }
        }

        public Object data3
        {
            get { return m_data3; }
            set { m_data3 = value; }
        }

        public Object data4
        {
            get { return m_data4; }
            set { m_data4 = value; }
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
            Assert.True(!Contains(del));
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

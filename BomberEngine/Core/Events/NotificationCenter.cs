using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;
using BomberEngine.Debugging;
using BomberEngine.Game;

namespace BomberEngine.Core.Events
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
            m_notificatoinsPool = new ObjectsPool<Notification>(timerManager);
        }

        public void Destroy()
        {
            CancelScheduledPosts();
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

        public void Post(String name, Object data)
        {
            NotificationDelegateList list = FindList(name);
            if (list != null && list.Count() > 0)
            {
                Notification notification = m_notificatoinsPool.NextObject();
                notification.Init(name, data);

                SchedulePost(notification);
            }
        }

        public void PostImmediately(String name, Object data)
        {   
            NotificationDelegateList list = FindList(name);
            if (list != null && list.Count() > 0)
            {
                Notification notification = m_notificatoinsPool.NextObject();
                notification.Init(name, data);

                list.NotifyDelegates(notification);

                m_notificatoinsPool.RecycleObject(notification);
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
            m_notificatoinsPool.RecycleObject(notification);
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
            Debug.Assert(notification != null);

            PostImmediately(notification);
        }
    }

    public class Notification : ObjectsPoolEntry<Notification>
    {
        private String m_name;
        private Object m_data;

        internal void Init(String name, Object data)
        {
            m_name = name;
            m_data = data;
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

        public bool boolData
        {
            get { return (bool)m_data; }
        }

        public int intData
        {
            get { return (int)m_data; }
        }

        public float floatData
        {
            get { return (float)m_data; }
        }

        public String stringData
        {
            get { return (String)m_data; }
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

using System;

namespace BomberEngine
{
    public abstract class BaseObject
    {
        protected void RegisterNotification(String name, NotificationDelegate del)
        {
            NotificationCenter().Register(name, del);
        }

        protected void UnregisterNotification(String name, NotificationDelegate del)
        {
            NotificationCenter().Unregister(name, del);
        }

        protected void UnregisterNotifications(NotificationDelegate del)
        {
            NotificationCenter().UnregisterAll(del);
        }

        protected void UnregisterNotifications()
        {
            NotificationCenter().UnregisterAll(this);
        }

        protected void PostNotification(String name, Object data = null, Object data2 = null, Object data3 = null, Object data4 = null)
        {
            NotificationCenter().Post(this, name, data, data2, data3, data4);
        }

        protected void PostNotificationImmediately(String name, Object data = null, Object data2 = null, Object data3 = null, Object data4 = null)
        {
            NotificationCenter().PostImmediately(this, name, data, data2, data3, data4);
        }

        protected virtual NotificationCenter NotificationCenter()
        {
            return Application.NotificationCenter();
        }
    }
}

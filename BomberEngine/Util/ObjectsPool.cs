using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Game;
using BomberEngine.Debugging;

namespace BomberEngine.Util
{
    public class ObjectsPool<T> : IDestroyable where T : ObjectsPoolEntry<T>, new()
    {
        private FastLinkedList<T> m_list;
        private FastLinkedList<T> m_recycleList; // put object here if it should be recycled later

        private TimerManager m_timerManager;

        public ObjectsPool()
            : this(Application.TimerManager())
        {
        }

        public ObjectsPool(TimerManager timerManager)
        {
            m_timerManager = timerManager;
            m_list = new FastLinkedList<T>();
        }

        public T NextObject()
        {
            T first = m_list.RemoveFirstItem();
            if (first != null)
            {
                return first;
            }
            return new T();
        }

        public void RecycleObject(T t)
        {
            t.RecycleObject();
            m_list.AddLastItem(t);
        }

        public void RecycleObjectLater(T t)
        {
            if (m_recycleList == null)
            {
                m_recycleList = new FastLinkedList<T>();
            }
            m_recycleList.AddLastItem(t);
            ScheduleRecycle();
        }

        public void Clear()
        {
            CancelRecycle();
            m_list.Clear();
            if (m_recycleList != null)
            {
                m_recycleList.Clear();
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Destroyable

        public void Destroy()
        {
            Clear();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Delayed recycle

        private void ScheduleRecycle()
        {
            m_timerManager.ScheduleOnce(RecycleObjectLaterCallback);
        }

        private void CancelRecycle()
        {
            m_timerManager.Cancel(RecycleObjectLaterCallback);
        }

        private void RecycleObjectLaterCallback(Timer timer)
        {
            Debug.Assert(m_recycleList != null && m_recycleList.size > 0);

            T t;
            while ((t = m_recycleList.RemoveFirstItem()) != null)
            {
                RecycleObject(t);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public int size
        {
            get { return m_list.size; }
        }

        #endregion
    }

    public class ObjectsPoolEntry<T> : FastLinkedListNode<T>
    {
        internal void RecycleObject()
        {
            OnRecycleObject();
        }

        protected virtual void OnRecycleObject()
        {
        }
    }
}

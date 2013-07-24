using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Game;
using BomberEngine.Debugging;

namespace BomberEngine.Util
{
    public class ObjectsPool<T> : IDestroyable where T : ObjectPoolEntry<T>, new()
    {
        private FastLinkedList<T> m_list;
        private FastLinkedList<T> m_recycleList; // put object here if it should be recycled later

        public ObjectsPool()
        {
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
            m_list.AddFirstItem(t);
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
            Application.ScheduleTimerOnce(RecycleObjectLaterCallback);
        }

        private void CancelRecycle()
        {
            Application.CancelTimer(RecycleObjectLaterCallback);
        }

        private void RecycleObjectLaterCallback(Timer timer)
        {
            Debug.Assert(m_recycleList != null && m_recycleList.size > 0);
            for (T t = m_recycleList.listFirst; t != null; )
            {
                T next = t.listNext;
                RecycleObject(t);
                t = next;
            }
            m_recycleList.Clear();
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

    public class ObjectPoolEntry<T> : FastLinkedListNode<T>
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

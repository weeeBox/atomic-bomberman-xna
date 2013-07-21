using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Core
{
    public abstract class BaseList<T> : IBaseCollection<T> where T : class
    {
        protected List<T> list;

        protected T nullElement;
        protected int removedCount;

        protected BaseList(T nullElement)
            : this(nullElement, 0)
        {
        }

        protected BaseList(T nullElement, int capacity)
            : this(new List<T>(capacity), nullElement)
        {   
        }

        protected BaseList(List<T> list, T nullElement)
        {
            this.list = list;
            this.nullElement = nullElement;
        }

        public virtual bool Add(T e)
        {
            list.Add(e);
            return true;
        }

        public virtual bool Remove(T e)
        {
            int index = list.IndexOf(e);
            if (index != -1)
            {
                Remove(index);
                return true;
            }

            return false;
        }

        public virtual T Get(int index)
        {
            return list[index];
        }

        public virtual int IndexOf(T e)
        {
            return list.IndexOf(e);
        }

        public virtual void Remove(int index)
        {
            ++removedCount;
            list[index] = nullElement;
        }

        public virtual void Clear()
        {
            list.Clear();
        }

        public virtual int Count()
        {
            return list.Count - removedCount;
        }

        public virtual bool Contains(T e)
        {
            return list.Contains(e);
        }

        public virtual bool IsNull()
        {
            return false;
        }

        protected void ClearRemoved()
        {
            for (int i = 0; removedCount > 0 && i < list.Count; ++i)
            {
                if (list[i] == nullElement)
                {
                    list.RemoveAt(i);
                    --removedCount;
                }
            }
        }
    }
}
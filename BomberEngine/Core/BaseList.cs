using System.Collections.Generic;

namespace BomberEngine
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
            Assert.True(!list.Contains(e));
            list.Add(e);
            return true;
        }

        public virtual bool Remove(T e)
        {
            int index = list.IndexOf(e);
            if (index != -1)
            {
                RemoveAt(index);
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

        public virtual void RemoveAt(int index)
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
            for (int i = list.Count - 1; removedCount > 0 && i >= 0; --i)
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
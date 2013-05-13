using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Core
{
    public abstract class BaseUpdatableList<T> : IUpdatable, Collection<T> where T : class, IUpdatable
    {   
        protected List<T> list;

        private T nullElement;
        private int removedCount;

        protected BaseUpdatableList()
        {
        }

        protected BaseUpdatableList(T nullElement)
            : this(nullElement, 0)
        {
        }

        protected BaseUpdatableList(T nullElement, int capacity)
        {   
            this.nullElement = nullElement;
            list = new List<T>(capacity);
        }

        public virtual void Update(float delta)
        {
            int elementsCount = list.Count;
            for (int i = 0; i < elementsCount; ++i) // do not update added items on that tick
            {
                list[i].Update(delta);
            }

            if (removedCount > 0)
            {
                ClearRemoved();
            }
        }

        public virtual bool Add(T updatable)
        {
            list.Add(updatable);
            return true;
        }

        public virtual bool Remove(T updatable)
        {
            int index = list.IndexOf(updatable);
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

        public virtual int IndexOf(T updatable)
        {
            return list.IndexOf(updatable);
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

        public virtual bool Contains(T updatable)
        {
            return list.Contains(updatable);
        }

        public virtual bool IsNull()
        {
            return false;
        }

        private void ClearRemoved()
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i] == nullElement)
                {
                    list.RemoveAt(i);
                    --removedCount;

                    if (removedCount == 0)
                    {
                        break;
                    }
                }
            }
        }
    }
}

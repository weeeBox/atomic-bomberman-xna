using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace util
{
    public class WeakList<T> : IList<T>
    {
        List<WeakReference> list;

        int count;

        public WeakList()
        {
            list = new List<WeakReference>();
        }

        public WeakList(int capacity)
        {
            list = new List<WeakReference>(capacity);
        }

        public int IndexOf(T item)
        {
            if (item != null)
            {
                for (int i = 0; i < Count; ++i)
                {
                    T t = get(i);
                    if (t != null && t.Equals(item))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            
        }

        public T this[int index]
        {
            get
            {
                if (index >= Count || index < 0)
                {
                    throw new IndexOutOfRangeException("Index " + index + " out of bounds 0.." + (Count - 1));
                }

                return get(index);
            }
            set
            {
                if (index >= Count || index < 0)
                {
                    throw new IndexOutOfRangeException("Index " + index + " out of bounds 0.." + (Count - 1));
                }

                set(value, index);
            }
        }

        public void Add(T item)
        {
            
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index == -1)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private T get(int index)
        {
            return (T) list[index].Target;
        }

        private void set(T item, int index)
        {
            list[index].Target = item;
        }
    }
}

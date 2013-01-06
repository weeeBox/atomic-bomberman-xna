using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Util
{
    public class ObjectsList<T> : IList<T>
    {
        private List<T> list;

        public ObjectsList()
        {
            list = new List<T>();
        }

        public ObjectsList(int capacity)
        {
            list = new List<T>(capacity);
        }

        public int IndexOf(T item)
        {
            return item != null ? list.IndexOf(item) : -1;
        }

        public void Insert(int index, T item)
        {
            CheckItem(item);
            list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                CheckItem(value);
                list[index] = value;
            }
        }

        public void Add(T item)
        {
            CheckItem(item);
            list.Add(item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(T item)
        {
            return item != null && list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return item != null && list.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        private void CheckItem(T item)
        {
            if (item == null)
            {
                throw new NullReferenceException("Item cannot be null");
            }
        }
    }
}

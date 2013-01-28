using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Util
{
    public class ConcurrentList<T> : IEnumerable<T>, IEnumerator<T>
    {
        private LinkedList<T> list;

        private LinkedListNode<T> nextNode;
        private LinkedListNode<T> lastNode;

        private bool isFirstNode;

        public ConcurrentList()
        {
            list = new LinkedList<T>();
        }

        public void Add(T item)
        {
            list.AddLast(item);
        }

        public bool Remove(T t)
        {
            LinkedListNode<T> node = list.Find(t);
            if (node != null)
            {
                if (node == nextNode)
                {
                    MoveNext();
                }
                list.Remove(node);
                return true;
            }
            return false;
        }

        public int Count()
        {
            return list.Count;
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(T T)
        {
            return list.Contains(T);
        }

        //////////////////////////////////////////////////////////////////////////////

        public T Current
        {
            get { return nextNode.Value; }
        }

        public void Dispose()
        {
            nextNode = null;
            lastNode = null;
        }

        object System.Collections.IEnumerator.Current
        {
            get { return nextNode.Value; }
        }

        public bool MoveNext()
        {
            if (nextNode == null)
            {
                if (isFirstNode)
                {
                    isFirstNode = false;
                    nextNode = list.First;
                }
            }
            else
            {
                nextNode = nextNode != lastNode ? nextNode.Next : null; // don't let to iterate throw newly added items
            }

            return nextNode != null;
        }

        public void Reset()
        {
            nextNode = null;
            lastNode = list.Last;
            isFirstNode = true;
        }

        //////////////////////////////////////////////////////////////////////////////

        public IEnumerator<T> GetEnumerator()
        {
            Reset();
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            Reset();
            return this;
        }
    }
}

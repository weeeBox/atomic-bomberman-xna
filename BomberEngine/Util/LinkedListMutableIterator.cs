using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Util
{
    public class LinkedListMutableIterator<T>
    {
        private LinkedList<T> list;
        private LinkedListNode<T> currentNode;

        public LinkedListMutableIterator(LinkedList<T> list)
        {
            this.list = list;
        }

        public LinkedListMutableIterator()
        {
            list = new LinkedList<T>();
        }

        public void Reset()
        {
            currentNode = list.First;
        }

        public bool HasNext()
        {
            return currentNode != null;
        }

        public T Next()
        {
            T item = currentNode.Value;
            currentNode = currentNode.Next;
            return item;
        }

        public bool Contains(T value)
        {
            return list.Contains(value);
        }

        public void Add(T value)
        {
            list.AddLast(value);
        }

        public void Remove(T value)
        {
            LinkedListNode<T> node = list.Find(value);
            if (currentNode == node)
            {
                currentNode = currentNode.Next;
            }
            list.Remove(node);
        }
    }
}

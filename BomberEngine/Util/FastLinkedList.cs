using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;

namespace BomberEngine.Util
{
    public class FastLinkedList<T> where T : ListNode<T>
    {
        public T listFirst;
        public T listLast;

        private int size;

        public void AddFirst(T item)
        {
            Insert(item, null, listFirst);
        }

        public void AddLast(T item)
        {
            Insert(item, listLast, null);
        }

        public void InsertBefore(T node, T item)
        {
            Insert(item, node != null ? node.listPrev : null, node);
        }

        public void InsertAfter(T node, T item)
        {
            Insert(item, node, node != null ? node.listNext : null);
        }

        public void Remove(T item)
        {
            Debug.Assert(size > 0);

            T prev = item.listPrev;
            T next = item.listNext;

            if (prev != null)
            {
                prev.listNext = next;
            }
            else
            {
                listFirst = next;
            }

            if (next != null)
            {
                next.listPrev = prev;
            }
            else
            {
                listLast = prev;
            }

            item.listNext = item.listPrev = null;
            --size;
        }

        public T RemoveFirst()
        {
            T node = listFirst;
            if (node != null)
            {   
                Remove(node);
            }

            return node;
        }

        public T RemoveLast()
        {
            T node = listLast;
            if (node != null)
            {
                Remove(node);
            }

            return node;
        }

        public bool Contains(T item)
        {
            for (T t = listFirst; t != null; t = t.listNext)
            {
                if (t == item)
                {
                    return true;
                }
            }

            return false;
        }

        private void Insert(T item, T prev, T next)
        {
            if (next != null)
            {
                next.listPrev = item;
            }
            else
            {
                listLast = item;
            }

            if (prev != null)
            {
                prev.listNext = item;
            }
            else
            {
                listFirst = item;
            }

            item.listPrev = prev;
            item.listNext = next;
            ++size;
        }

        public int GetListSize()
        {
            return size;
        }
    }
}

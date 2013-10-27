
namespace BomberEngine
{
    public interface IFastLinkedList // marker interface: we need to store a list instance inside ListNode class, but generics don't allow that
    {
    }

    public class FastLinkedList<T> : IFastLinkedList where T : FastLinkedListNode<T>
    {
        private T m_listFirst;
        private T m_listLast;

        private int m_size;

        public void AddFirstItem(T item)
        {
            InsertItem(item, null, m_listFirst);
        }

        public void AddLastItem(T item)
        {
            InsertItem(item, m_listLast, null);
        }

        public void InsertBeforeItem(T node, T item)
        {
            InsertItem(item, node != null ? node.m_listPrev : null, node);
        }

        public void InsertAfterItem(T node, T item)
        {
            InsertItem(item, node, node != null ? node.m_listNext : null);
        }

        public void RemoveItem(T item)
        {
            Debug.Assert(m_size > 0);
            Debug.Assert(item.m_list == this);

            T prev = item.m_listPrev;
            T next = item.m_listNext;

            if (prev != null)
            {
                prev.m_listNext = next;
            }
            else
            {
                m_listFirst = next;
            }

            if (next != null)
            {
                next.m_listPrev = prev;
            }
            else
            {
                m_listLast = prev;
            }

            item.m_listNext = item.m_listPrev = null;
            item.m_list = null;
            --m_size;
        }

        public T RemoveFirstItem()
        {
            T node = m_listFirst;
            if (node != null)
            {
                RemoveItem(node);
            }

            return node;
        }

        public T RemoveLastItem()
        {
            T node = m_listLast;
            if (node != null)
            {
                RemoveItem(node);
            }

            return node;
        }

        public bool ContainsItem(T item)
        {
            if (item.m_list != this)
            {
                return false;
            }

            for (T t = m_listFirst; t != null; t = t.m_listNext)
            {
                if (t == item)
                {
                    return true;
                }
            }

            return false;
        }

        private void InsertItem(T item, T prev, T next)
        {
            Debug.Assert(item.m_list == null);

            if (next != null)
            {
                next.m_listPrev = item;
            }
            else
            {
                m_listLast = item;
            }

            if (prev != null)
            {
                prev.m_listNext = item;
            }
            else
            {
                m_listFirst = item;
            }

            item.m_listPrev = prev;
            item.m_listNext = next;
            item.m_list = this;
            ++m_size;
        }

        public void Clear()
        {
            for (T t = m_listFirst; t != null; )
            {
                T next = t.listNext;
                t.m_listPrev = t.m_listNext = null;
                t.m_list = null;
                t = next;
            }

            m_listFirst = m_listLast = null;
            m_size = 0;
        }

        public int size
        {
            get { return m_size; }
        }

        public T listFirst
        {
            get { return m_listFirst; }
            protected set { m_listFirst = value; }
        }

        public T listLast
        {
            get { return m_listLast; }
            protected set { m_listLast = value; }
        }
    }

    public class FastLinkedListNode<T>
    {
        internal T m_listPrev;
        internal T m_listNext;
        internal IFastLinkedList m_list;

        public T listPrev
        {
            get { return m_listPrev; }
            protected set { m_listPrev = value; }
        }

        public T listNext
        {
            get { return m_listNext; }
            protected set { m_listNext = value; }
        }
    }
}

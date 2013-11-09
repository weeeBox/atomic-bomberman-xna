
namespace BomberEngine
{
    internal interface IFastList
    {
    }

    public class FastList<T> : IFastList where T : FastListNode
    {
        internal FastListNode m_listFirst;
        internal FastListNode m_listLast;

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
            Assert.True(m_size > 0);
            Assert.True(item.m_list == this);

            FastListNode prev = item.m_listPrev;
            FastListNode next = item.m_listNext;

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
            T node = listFirst;
            if (node != null)
            {
                RemoveItem(node);
            }

            return node;
        }

        public T RemoveLastItem()
        {
            T node = listLast;
            if (node != null)
            {
                RemoveItem(node);
            }

            return node;
        }

        public bool ContainsItem(FastListNode item)
        {
            if (item.m_list != this)
            {
                return false;
            }

            for (FastListNode t = m_listFirst; t != null; t = t.m_listNext)
            {
                if (t == item)
                {
                    return true;
                }
            }

            return false;
        }

        private void InsertItem(FastListNode item, FastListNode prev, FastListNode next)
        {
            Assert.True(item.m_list == null);

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
            for (FastListNode t = m_listFirst; t != null; )
            {
                FastListNode next = t.m_listNext;
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
            get { return (T)m_listFirst; }
            protected set { m_listFirst = value; }
        }

        public T listLast
        {
            get { return (T)m_listLast; }
            protected set { m_listLast = value; }
        }
    }

    public class FastListNode
    {
        internal FastListNode m_listPrev;
        internal FastListNode m_listNext;
        internal IFastList m_list;

        protected FastListNode listPrev
        {
            get { return m_listPrev; }
        }

        protected FastListNode listNext
        {
            get { return m_listNext; }
        }
    }
}

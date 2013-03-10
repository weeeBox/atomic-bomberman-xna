using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Util
{
    public class FastLinkedList<T> where T : ListNode<T>
    {
        public T listFirst;
        public T listLast;

        private int size;

        public void AddFirst(T item)
        {
            item.listPrev = null;
            item.listNext = listFirst;

            if (listFirst != null)
            {
                listFirst.listPrev = item;
            }
            else
            {
                listLast = item;
            }

            listFirst = item;
            ++size;
        }

        public void AddLast(T item)
        {
            item.listPrev = listLast;
            item.listNext = null;

            if (listLast != null)
            {
                listLast.listNext = item;
            }
            else
            {
                listFirst = item;
            }

            listLast = item;
            ++size;
        }

        public int GetListSize()
        {
            return size;
        }
    }
}

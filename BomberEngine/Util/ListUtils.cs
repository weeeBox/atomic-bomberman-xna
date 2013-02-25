using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Util
{
    public class ListUtils
    {
        /* Returns new head */
        public static T Add<T>(T head, T item) where T : ListNode<T>
        {
            if (head != null)
            {
                head.listPrev = item;
            }

            item.listPrev = null;
            item.listNext = head;
            head = item;

            return item;
        }

        /* Returns new head */
        public static T Remove<T>(T head, T item) where T : ListNode<T>
        {
            T prev = item.listPrev;
            T next = item.listNext;

            if (prev != null)
            {
                prev.listNext = next;
            }
            else
            {
                head = next;
            }

            if (next != null)
            {
                next.listPrev = prev;
            }

            return head;
        }
    }
}

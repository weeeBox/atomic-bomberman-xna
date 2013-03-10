using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Util
{
    public class FastLinkedListIterator<T> where T : ListNode<T>
    {
        public T current;

        public bool HasNext()
        {
            return current != null;
        }

        public T Next()
        {
            T item = current;
            current = current.listNext;
            return item;
        }

        internal void OnItemRemove(T item)
        {
            if (item == current)
            {
                current = item.listNext;
            }
        }
    }
}

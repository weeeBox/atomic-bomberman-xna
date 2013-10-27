using System;
using System.Collections.Generic;

namespace BomberEngine
{
    public class CConsoleHistory
    {
        private LinkedList<String> list;
        private LinkedListNode<String> lastNode;

        private int capacity;

        private bool hitFirst;
        private bool hitLast;

        public CConsoleHistory(int capacity)
        {
            this.capacity = capacity;
            list = new LinkedList<String>();
        }

        public void Push(String line)
        {
            for (LinkedListNode<String> node = list.First; node != null; node = node.Next)
            {
                if (node.Value.Equals(line))
                {
                    list.Remove(node);
                    break;
                }
            }

            if (list.Count == capacity)
            {
                list.RemoveFirst();
            }

            list.AddLast(line);

            lastNode = null;
            hitFirst = hitLast = false;
        }

        public String Next()
        {
            if (lastNode != null)
            {   
                String line = lastNode.Value;

                LinkedListNode<String> prevNode = lastNode.Previous;
                LinkedListNode<String> nextNode = lastNode.Next;
                hitFirst = prevNode == null;
                hitLast = nextNode == null;

                if (!hitLast)
                {
                    lastNode = nextNode;
                }
                
                return hitFirst ? Next() : line;
            }

            return null;
        }

        public String Prev()
        {
            if (lastNode != null)
            {
                String line = lastNode.Value;

                LinkedListNode<String> prevNode = lastNode.Previous;
                LinkedListNode<String> nextNode = lastNode.Next;
                hitFirst = prevNode == null;
                hitLast = nextNode == null;

                if (!hitFirst)
                {
                    lastNode = prevNode;
                }

                return hitLast ? Prev() : line;
            }

            if (list.Count > 0)
            {
                lastNode = list.Last;
                return lastNode.Value;
            }

            return null;
        }
    }
}

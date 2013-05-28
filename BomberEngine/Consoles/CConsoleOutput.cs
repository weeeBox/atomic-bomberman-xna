using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Consoles
{
    public class CConsoleOutput
    {
        private LinkedList<String> list;
        private int capacity;

        public LinkedListNode<String> lastNode;

        public CConsoleOutput(int capacity)
        {
            this.capacity = capacity;
            list = new LinkedList<String>();
        }

        public void Push(String line)
        {   
            if (list.Count == capacity)
            {
                list.RemoveFirst();
            }

            lastNode = list.AddLast(line);
        }
    }
}

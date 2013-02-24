using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Cells
{
    public class CellIterator
    {
        public FieldCell currentCell;

        private static CellIterator root;
        private CellIterator prev;
        private CellIterator next;

        private bool valid;

        private CellIterator(FieldCell cell)
        {
            Init(cell);
        }

        private void Init(FieldCell cell)
        {
            currentCell = cell;
            valid = true;
        }

        public static CellIterator Create(FieldCell cell)
        {
            if (root == null) // create the first element
            {
                root = new CellIterator(cell);
                return root;
            }

            // try to find an invalid iterator
            for (CellIterator iter = root; iter != null; iter = iter.next)
            {
                if (!iter.valid)
                {
                    iter.Init(cell);
                    return iter;
                }
            }

            // create new iterator
            CellIterator newIter = new CellIterator(cell);
            newIter.next = root;
            root.prev = newIter;
            root = newIter;

            return newIter;
        }

        public bool HasNext()
        {
            return currentCell != null;
        }

        public FieldCell Next()
        {
            FieldCell cell = currentCell;
            currentCell = currentCell.listNext;
            return cell;
        }

        public void Destroy()
        {
            currentCell = null;
            valid = false;

            if (this != root)
            {
                // move from the current pos
                if (prev != null)
                {
                    prev.next = next;
                }

                if (next != null)
                {
                    next.prev = prev;
                }

                // insert in the beggining
                prev = null;
                next = root;
                root.prev = this;
                root = this;
            }
        }

        internal static void CellRemoved(FieldCell cell)
        {
            for (CellIterator iter = root; iter != null; iter = iter.next)
            {
                if (iter.currentCell == cell)
                {
                    iter.currentCell = cell.listNext;
                }
            }
        }
    }
}

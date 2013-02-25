using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldCellIterator
    {
        public FieldCell currentCell;

        private static FieldCellIterator root;
        private FieldCellIterator prev;
        private FieldCellIterator next;

        private bool valid;

        private FieldCellIterator(FieldCell cell)
        {
            Init(cell);
        }

        private void Init(FieldCell cell)
        {
            currentCell = cell;
            valid = true;
        }

        public static FieldCellIterator Create(FieldCell cell)
        {
            if (root == null) // create the first element
            {
                root = new FieldCellIterator(cell);
                return root;
            }

            // try to find an invalid iterator
            for (FieldCellIterator iter = root; iter != null; iter = iter.next)
            {
                if (!iter.valid)
                {
                    iter.Init(cell);
                    return iter;
                }
            }

            // create new iterator
            FieldCellIterator newIter = new FieldCellIterator(cell);
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
            for (FieldCellIterator iter = root; iter != null; iter = iter.next)
            {
                if (iter.currentCell == cell)
                {
                    iter.currentCell = cell.listNext;
                }
            }
        }
    }
}

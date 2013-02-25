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

        private static FieldCellIterator freeRoot;

        private FieldCellIterator prev;
        private FieldCellIterator next;

        private bool used;

        private FieldCellIterator(FieldCell cell)
        {
            Init(cell);
        }

        private void Init(FieldCell cell)
        {
            currentCell = cell;
            used = true;
        }

        public static FieldCellIterator Create(FieldCell cell)
        {
            if (freeRoot == null) // create the first element
            {
                freeRoot = new FieldCellIterator(cell);
                return freeRoot;
            }

            // try to find an invalid iterator
            for (FieldCellIterator iter = freeRoot; iter != null; iter = iter.next)
            {
                if (!iter.used)
                {
                    iter.Init(cell);
                    return iter;
                }
            }

            // create new iterator
            FieldCellIterator newIter = new FieldCellIterator(cell);
            newIter.next = freeRoot;
            freeRoot.prev = newIter;
            freeRoot = newIter;

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
            used = false;

            if (this != freeRoot)
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
                next = freeRoot;
                freeRoot.prev = this;
                freeRoot = this;
            }
        }

        internal static void CellRemoved(FieldCell cell)
        {
            for (FieldCellIterator iter = freeRoot; iter != null; iter = iter.next)
            {
                if (iter.currentCell == cell)
                {
                    iter.currentCell = cell.listNext;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldCellIterator
    {
        private FieldCellSlot slot;
        private FieldCell currentCell;

        private static FieldCellIterator freeRoot;

        internal FieldCellIterator prev;
        internal FieldCellIterator next;

        public FieldCellIterator(FieldCellSlot slot, FieldCell cell)
        {
            Init(slot, cell);
        }

        private void Init(FieldCellSlot slot, FieldCell cell)
        {
            this.slot = slot;
            currentCell = cell;
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

        public static FieldCellIterator Create(FieldCellSlot slot, FieldCell cell)
        {
            if (freeRoot == null) // create the first element
            {
                freeRoot = new FieldCellIterator(slot, cell);
                return freeRoot;
            }

            // try to get a free iterator
            FieldCellIterator iter = RemoveFromFreeList();
            if (iter != null)
            {
                iter.Init(slot, cell);
                return iter;
            }

            // create new iterator
            iter = new FieldCellIterator(slot, cell);
            iter.next = freeRoot;
            freeRoot.prev = iter;
            freeRoot = iter;

            return iter;
        }

        public void Destroy()
        {
            slot.RemoveIterator(this);
            slot = null;
            currentCell = null;

            AddToFreeList(this);
        }

        private static FieldCellIterator RemoveFromFreeList()
        {   
            FieldCellIterator iter = freeRoot;
            freeRoot = freeRoot.next;
            if (freeRoot != null)
            {
                freeRoot.prev = null;
            }
            iter.prev = iter.next = null;
            return iter;
        }

        private void AddToFreeList(FieldCellIterator iter)
        {
            if (freeRoot != null)
            {
                freeRoot.prev = iter;
            }
            iter.prev = null;
            iter.next = freeRoot;
            freeRoot = iter;
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

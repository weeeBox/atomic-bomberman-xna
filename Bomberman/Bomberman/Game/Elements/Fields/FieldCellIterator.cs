using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;
using BomberEngine.Util;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldCellIterator : ListNode<FieldCellIterator>
    {
        private FieldCellSlot slot;
        private FieldCell currentCell;

        private static FieldCellIterator freeRoot;

        public FieldCellIterator(FieldCellSlot slot, FieldCell cell)
        {
            Init(slot, cell);
        }

        private void Init(FieldCellSlot slot, FieldCell cell)
        {
            if (slot == null)
            {
                throw new ArgumentException("Slot is null");
            }

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

        public void Destroy()
        {
            slot.RemoveIterator(this);
            slot = null;
            currentCell = null;

            AddToFreeList(this);
        }

        internal void CellRemoved(FieldCell cell)
        {
            if (currentCell == cell)
            {
                currentCell = cell.listNext;
            }
        }

        internal static FieldCellIterator Create(FieldCellSlot slot, FieldCell cell)
        {
            // try to get a free iterator
            FieldCellIterator iter = RemoveFromFreeList();
            if (iter != null)
            {
                iter.Init(slot, cell);
                return iter;
            }

            // create new iterator
            return new FieldCellIterator(slot, cell);
        }

        private static FieldCellIterator RemoveFromFreeList()
        {
            if (freeRoot != null)
            {
                FieldCellIterator iter = freeRoot;
                freeRoot = ListUtils.Remove(freeRoot, iter);
                return iter;
            }
            return null;
        }

        private void AddToFreeList(FieldCellIterator iter)
        {   
            freeRoot = ListUtils.Add(freeRoot, iter);
        }
    }
}

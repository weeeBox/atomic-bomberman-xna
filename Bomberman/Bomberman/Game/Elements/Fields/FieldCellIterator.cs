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
            // try to get a free iterator
            FieldCellIterator iter = RemoveFromFreeList();
            if (iter != null)
            {
                iter.Init(slot, cell);
                return iter;
            }

            // create new iterator
            iter = new FieldCellIterator(slot, cell);
            freeRoot = ListUtils.Add(freeRoot, iter);

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
            freeRoot = ListUtils.Remove(freeRoot, iter);
            return iter;
        }

        private void AddToFreeList(FieldCellIterator iter)
        {   
            freeRoot = ListUtils.Add(freeRoot, iter);
        }
    }
}

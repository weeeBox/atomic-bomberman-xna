using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;
using BombermanCommon.Resources.Scheme;
using Bomberman.Game.Elements.Cells;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldCellArray
    {
        private int width;
        private int height;

        private FieldCellSlot[] slots;

        public FieldCellArray(int width, int height)
        {
            this.width = width;
            this.height = height;

            slots = new FieldCellSlot[width * height];
            for (int i = 0; i < slots.Length; ++i)
            {
                slots[i] = new FieldCellSlot();
            }
        }

        public FieldCellSlot[] GetSlots()
        {
            return slots;
        }

        public FieldCellSlot Get(int x, int y)
        {
            int index = GetSlotIndex(x, y);
            return slots[index];
        }

        public bool Remove(FieldCell cell)
        {
            int slotIndex = cell.slotIndex;
            if (slotIndex != -1)
            {
                slots[slotIndex].RemoveCell(cell);
                cell.slotIndex = -1;

                return true;
            }

            return false;
        }

        public void Add(int x, int y, FieldCell cell)
        {
            Remove(cell);

            int slotIndex = GetSlotIndex(x, y);
            slots[slotIndex].AddCell(cell);
            cell.slotIndex = slotIndex;
        }

        public bool Contains(FieldCell cell)
        {
            int slotIndex = GetSlotIndex(cell.cx, cell.cy);
            return slots[slotIndex].Contains(cell);
        }

        public int CellsCount(FieldCell root)
        {
            int count = 0;
            for (FieldCell c = root; c != null; c = c.listNext)
            {
                ++count;
            }

            return count;
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }

        private int GetSlotIndex(int x, int y)
        {
            return y * width + x;
        }
    }
}

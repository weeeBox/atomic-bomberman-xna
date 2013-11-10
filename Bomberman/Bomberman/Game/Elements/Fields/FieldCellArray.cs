using BomberEngine;

namespace Bomberman.Gameplay.Elements.Fields
{
    public class FieldCellArray : IResettable
    {
        public int width;
        public int height;

        public FieldCellSlot[] slots;

        public FieldCellArray(int width, int height)
        {
            this.width = width;
            this.height = height;

            slots = new FieldCellSlot[width * height];

            int index = 0;
            for (int cy = 0; cy < height; ++cy)
            {
                for (int cx = 0; cx < width; ++cx)
                {
                    slots[index++] = new FieldCellSlot(cx, cy);
                }
            }
        }

        public void Reset()
        {
            for (int i = 0; i < slots.Length; ++i)
            {
                slots[i].Reset();
            }
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

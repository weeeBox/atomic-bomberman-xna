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

        private FieldCell[] cells;

        public FieldCellArray(int width, int height)
        {
            this.width = width;
            this.height = height;

            cells = new FieldCell[width * height];
        }

        public FieldCell[] GetArray()
        {
            return cells;
        }

        public FieldCell Get(int x, int y)
        {
            int index = GetIndex(x, y);
            return index != -1 ? cells[index] : null;
        }

        public void Add(int x, int y, FieldCell cell)
        {
            int index = GetIndex(x, y);
            Debug.Assert(index != -1, "Trying to set a cell outside of a field: x=" + x + " y=" + y);
            cells[index] = Add(index, cell);
        }

        public void Remove(int x, int y, FieldCell cell)
        {
            int index = GetIndex(x, y);
            Debug.Assert(index != -1, "Trying to set a cell outside of a field: x=" + x + " y=" + y);
            cells[index] = Remove(index, cell);
        }

        private FieldCell Add(int index, FieldCell cell)
        {
            FieldCell root = cells[index];
            if (root != null)
            {
                root.listNext = cell;
                cell.listPrev = root;
            }

            return cell;
        }

        private FieldCell Remove(int index, FieldCell cell)
        {
            FieldCell root = cells[index];
            if (cell == root)
            {
                Debug.Assert(cell.listPrev == null);
                return cell.listNext;
            }

            FieldCell prev = cell.listPrev;
            FieldCell next = cell.listNext;

            prev.listNext = next;
            next.listPrev = prev;

            return root;
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }

        private int GetIndex(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                return y * width + x;
            }
            return -1;
        }
    }
}

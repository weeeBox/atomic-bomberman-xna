using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldCellArray
    {
        private int width;
        private int height;

        private FieldCell[] cells;

        public FieldCellArray(int width, int height)
        {
            Debug.CheckArgument(width > 0, "Invalid width value: " + width);
            Debug.CheckArgument(height > 0, "Invalid height value: " + height);

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
            return cells[index];
        }

        public void Set(int x, int y, FieldCell cell)
        {
            int index = GetIndex(x, y);
            cells[index] = cell;
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
            Debug.CheckArgumentRange("x", x, 0, width);
            Debug.CheckArgumentRange("y", y, 0, height);

            return y * width + x;
        }
    }
}

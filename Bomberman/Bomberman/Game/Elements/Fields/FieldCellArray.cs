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

        public FieldCellArray(FieldData data)
        {
            width = data.GetWidth();
            height = data.GetHeight();

            FieldBlocks[] blockData = data.GetDataArray();
            cells = new FieldCell[blockData.Length];

            int index = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    FieldBlocks block = blockData[index];
                    switch (block)
                    {
                        case FieldBlocks.Blank:
                        {
                            cells[index] = new EmptyCell(x, y);
                            break;
                        }

                        case FieldBlocks.Brick:
                        {
                            cells[index] = new BrickCell(x, y, false);
                            break;
                        }

                        case FieldBlocks.Solid:
                        {
                            cells[index] = new BrickCell(x, y, true);
                            break;
                        }
                    }

                    ++index;
                }
            }
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

        public void Set(int x, int y, FieldCell cell)
        {
            int index = GetIndex(x, y);
            Debug.Assert(index != -1, "Trying to set a cell outside of a field: x=" + x + " y=" + y);
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
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                return y * width + x;
            }
            return -1;
        }
    }
}

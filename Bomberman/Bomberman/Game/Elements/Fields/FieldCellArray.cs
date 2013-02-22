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
            Add(index, cell);
        }

        public void Remove(int x, int y, FieldCell cell)
        {
            int index = GetIndex(x, y);
            Debug.Assert(index != -1, "Trying to set a cell outside of a field: x=" + x + " y=" + y);
            Remove(index, cell);
        }

        public bool HasCell(int x, int y)
        {
            int index = GetIndex(x, y);
            return index != -1 && cells[index] != null;
        }

        private void Add(int index, FieldCell cell)
        {   
            FieldCell nextCell = cells[index];
            if (nextCell == null)
            {
                cells[index] = cell;
                cell.listNext = null;
                cell.listPrev = null;
            }
            else
            {
                // find insertion point
                FieldCell lastCell = null;
                for (; nextCell != null; nextCell = nextCell.listNext)
                {
                    if (nextCell.listPriority <= cell.listPriority)
                    {
                        break;
                    }

                    lastCell = nextCell; // save last cell to insert at the end
                }

                if (nextCell == null) // insert at the end
                {
                    Debug.Assert(lastCell.listNext == null);
                    lastCell.listNext = cell;
                    cell.listPrev = lastCell;
                    cell.listNext = null;
                }
                else
                {
                    FieldCell prevCell = nextCell.listPrev;
                    if (prevCell != null)
                    {
                        prevCell.listNext = cell;
                    }
                    else
                    {
                        cells[index] = cell;
                    }
                    
                    cell.listPrev = prevCell;
                    cell.listNext = nextCell;
                    nextCell.listPrev = cell;
                }
            }
        }

        private void Remove(int index, FieldCell cell)
        {
            FieldCell prevCell = cell.listPrev;
            FieldCell nextCell = cell.listNext;

            cell.listNext = cell.listPrev = null;

            if (prevCell != null)
            {
                prevCell.listNext = nextCell;
            }
            else
            {
                cells[index] = nextCell;
            }

            if (nextCell != null)
            {
                nextCell.listPrev = prevCell;
            }
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

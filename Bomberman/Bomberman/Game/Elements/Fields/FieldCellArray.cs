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
            return cells[index];
        }

        public void Add(int x, int y, FieldCell cell)
        {
            int index = GetIndex(x, y);
            Add(index, cell);
        }

        public void Remove(FieldCell cell)
        {
            if (cell.listIndex != -1)
            {
                Remove(cell.listIndex, cell);
            }
        }

        public bool HasCell(int x, int y)
        {
            int index = GetIndex(x, y);
            return cells[index] != null;
        }

        private void Add(int index, FieldCell cell)
        {
            Remove(cell.listIndex, cell);
            cell.listIndex = index;

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

        private bool Remove(int index, FieldCell cell)
        {
            if (cell.listIndex != -1)
            {
                FieldCell prevCell = cell.listPrev;
                FieldCell nextCell = cell.listNext;

                CellIterator.CellRemoved(cell);

                cell.listNext = cell.listPrev = null;
                cell.listIndex = -1;

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

                return true;
            }

            return false;
        }

        public bool Contains(FieldCell cell)
        {
            int index = GetIndex(cell.cx, cell.cy);
            return Contains(index, cell);
        }

        public bool Contains(int index, FieldCell cell)
        {
            FieldCell root = cells[index];
            return Contains(root, cell);
        }

        public bool Contains(FieldCell root, FieldCell cell)
        {
            for (FieldCell c = root; c != null; c = c.listNext)
            {
                if (c == cell)
                {
                    return true;
                }
            }

            return false;
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

        private int GetIndex(int x, int y)
        {
            return y * width + x;
        }
    }
}

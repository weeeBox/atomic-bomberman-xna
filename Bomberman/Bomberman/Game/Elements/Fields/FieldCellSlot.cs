using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Players;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldCellSlot
    {
        public FieldCell[] cells;

        public FieldCellSlot()
        {
            int size = (int)FieldCellType.Count;
            cells = new FieldCell[size];
        }

        public void AddCell(FieldCell cell)
        {
            int index = GetIndex(cell);

            FieldCell nextCell = cells[index];
            if (nextCell == null)
            {
                cells[index] = cell;
                cell.listNext = null;
                cell.listPrev = null;
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

        public void RemoveCell(FieldCell cell)
        {   
            FieldCell prevCell = cell.listPrev;
            FieldCell nextCell = cell.listNext;

            CellIterator.CellRemoved(cell);

            cell.listNext = cell.listPrev = null;

            if (prevCell != null)
            {
                prevCell.listNext = nextCell;
            }
            else
            {
                int index = GetIndex(cell);
                cells[index] = nextCell;
            }

            if (nextCell != null)
            {
                nextCell.listPrev = prevCell;
            }
        }

        public bool ContainsSolid()
        {
            return Contains(FieldCellType.Solid);
        }

        public SolidCell GetSolid()
        {
            return (SolidCell)Get(FieldCellType.Solid);
        }

        public bool ContainsBrick()
        {
            return Contains(FieldCellType.Brick);
        }

        public BrickCell GetBrick()
        {
            return (BrickCell)Get(FieldCellType.Brick);
        }

        public bool ContainsPowerup()
        {
            return Contains(FieldCellType.Powerup);
        }

        public PowerupCell GetPowerup()
        {
            return (PowerupCell)Get(FieldCellType.Powerup);
        }

        public bool ContainsFlame()
        {
            return Contains(FieldCellType.Flame);
        }

        public FlameCell GetFlame()
        {
            return (FlameCell)Get(FieldCellType.Flame);
        }

        public bool ContainsBomb()
        {
            return Contains(FieldCellType.Bomb);
        }

        public Bomb GetBomb()
        {
            return (Bomb)Get(FieldCellType.Bomb);
        }

        public bool ContainsPlayer()
        {
            return Contains(FieldCellType.Player);
        }

        public Player GetPlayer()
        {
            return (Player)Get(FieldCellType.Player);
        }

        public bool ContainsObstacle()
        {
            foreach (FieldCell cell in cells)
            {
                if (cell != null && cell.IsObstacle())
                {
                    return true;
                }
            }

            return false;
        }

        public bool Contains(FieldCellType type)
        {
            return Get(type) != null;
        }

        public FieldCell Get(FieldCellType type)
        {
            int index = GetIndex(type);
            return cells[index];
        }

        private int GetIndex(FieldCell cell)
        {
            return (int)cell.type;
        }

        private int GetIndex(FieldCellType type)
        {
            return (int)type;
        }
    }
}

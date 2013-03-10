using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Players;
using BomberEngine;
using BomberEngine.Debugging;
using BomberEngine.Util;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldCellSlot : FastLinkedList<FieldCell>
    {
        private static FieldCellType[] STATIC_CELL_TYPES = 
        {
            FieldCellType.Solid,
            FieldCellType.Brick,
            FieldCellType.Powerup,
        };

        public int cx;
        public int cy;

        public FieldCellSlot(int cx, int cy)
        {
            this.cx = cx;
            this.cy = cy;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Add/Remove

        public void AddCell(FieldCell cell)
        {
            FieldCell node = FindInsertionNode(cell);
            if (node != null)
            {
                InsertAfterItem(node, cell);
            }
            else
            {
                AddFirstItem(cell);
            }
        }

        public void RemoveCell(FieldCell cell)
        {
            RemoveItem(cell);
        }

        private FieldCell FindInsertionNode(FieldCell cell)
        {
            int priority = cell.GetPriority();
            for (FieldCell c = listLast; c != null; c = c.listPrev)
            {
                if (c.GetPriority() <= priority)
                {
                    return c;
                }
            }

            return null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Getters/Setters

        public FieldCell Cell()
        {
            return listFirst;
        }

        public FieldCell Get(FieldCellType type)
        {
            if (!IsEmpty())
            {
                for (FieldCell cell = listFirst; cell != null; cell = cell.listNext)
                {
                    if (cell.type == type)
                    {
                        return cell;
                    }
                }
            }

            return null;
        }

        public int Size()
        {
            return GetListSize();
        }

        public bool IsEmpty()
        {
            return GetListSize() == 0;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

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
            if (!IsEmpty())
            {
                for (FieldCell c = listLast; c != null; c = c.listPrev)
                {
                    if (c.IsObstacle())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Contains(FieldCell cell)
        {
            return !IsEmpty() && ContainsItem(cell);
        }

        public FieldCell GetStaticCell()
        {
            for (int i = 0; i < STATIC_CELL_TYPES.Length; ++i)
            {
                FieldCell cell = Get(STATIC_CELL_TYPES[i]);
                if (cell != null)
                {
                    return cell;
                }
            }

            return null;
        }

        public bool Contains(FieldCellType type)
        {
            return Get(type) != null;
        }

        private int GetIndex(FieldCell cell)
        {
            return (int)cell.type;
        }

        private int GetIndex(FieldCellType type)
        {
            return (int)type;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Iterators
        #endregion
    }
}

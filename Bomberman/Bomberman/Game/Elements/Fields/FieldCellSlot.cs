using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Players;
using BomberEngine;
using BomberEngine.Debugging;
using BomberEngine.Util;
using BomberEngine.Core;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldCellSlot : IResettable
    {
        public int cx;
        public int cy;

        public FieldCell staticCell;
        public MovableCellList movableCells;

        public FieldCellSlot(int cx, int cy)
        {
            this.cx = cx;
            this.cy = cy;

            movableCells = new MovableCellList();
        }

        public void Reset()
        {
            staticCell = null;
            movableCells.Reset();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Add/Remove

        public void AddCell(FieldCell cell)
        {
            if (cell.IsMovable())
            {
                movableCells.Add(cell.AsMovable());
            }
            else
            {
                Debug.Assert(staticCell == null);
                staticCell = cell;
            }
        }

        public void RemoveCell(FieldCell cell)
        {
            if (cell.IsMovable())
            {
                movableCells.Remove(cell.AsMovable());
            }
            else
            {
                Debug.Assert(staticCell == cell);
                staticCell = null;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        public int Size()
        {
            return movableCells.Size() + (staticCell != null ? 1 : 0);
        }

        public int MovableCount()
        {
            return movableCells.Size();
        }

        //public bool ContainsSolid()
        //{
        //    return Contains(FieldCellType.Solid);
        //}

        public bool IsEmpty()
        {
            return staticCell == null && movableCells.Size() == 0;
        }

        public SolidCell GetSolid()
        {
            return staticCell != null ? staticCell.AsSolid() : null;
        }

        //public bool ContainsBrick()
        //{
        //    return Contains(FieldCellType.Brick);
        //}

        public BrickCell GetBrick()
        {
            return staticCell != null ? staticCell.AsBrick() : null;
        }

        //public bool ContainsPowerup()
        //{
        //    return Contains(FieldCellType.Powerup);
        //}

        public PowerupCell GetPowerup()
        {
            return staticCell != null ? staticCell.AsPowerup() : null;
        }

        //public bool ContainsFlame()
        //{
        //    return Contains(FieldCellType.Flame);
        //}

        public FlameCell GetFlame()
        {
            return staticCell != null ? staticCell.AsFlame() : null;
        }

        //public bool ContainsBomb()
        //{
        //    return Contains(FieldCellType.Bomb);
        //}

        public Bomb GetBomb()
        {
            LinkedList<MovableCell> list = movableCells.list;
            for (LinkedListNode<MovableCell> node = list.First; node != null; node = node.Next)
            {
                MovableCell cell = node.Value;
                if (cell.IsBomb())
                {
                    return cell.AsBomb();
                }
            }

            return null;
        }

        //public bool ContainsPlayer()
        //{
        //    return Contains(FieldCellType.Player);
        //}

        //public Player GetPlayer()
        //{
        //    return (Player)Get(FieldCellType.Player);
        //}

        public bool Contains(FieldCell cell)
        {   
            if (staticCell == cell)
            {
                return true;
            }

            if (cell.IsMovable() && movableCells.Contains(cell.AsMovable()))
            {
                return true;
            }

            return false;
        }

        public bool ContainsObstacle()
        {
            if (staticCell != null)
            {
                return staticCell.IsObstacle();
            }

            LinkedList<MovableCell> list = movableCells.list;
            for (LinkedListNode<MovableCell> node = list.First; node != null; node = node.Next)
            {
                MovableCell cell = node.Value;
                if (cell.IsObstacle())
                {
                    return true;
                }
            }

            return false;
        }

        public void GetCells(LinkedList<FieldCell> list)
        {
            if (staticCell != null)
            {
                list.AddLast(staticCell);
            }

            LinkedList<MovableCell> movableList = movableCells.list;
            foreach (FieldCell cell in movableList)
            {
                list.AddLast(cell);
            }
        }

        #endregion
    }
}

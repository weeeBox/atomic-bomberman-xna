using System.Collections.Generic;
using BomberEngine;
using Bomberman.Game.Elements.Cells;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldCellSlot : IResettable
    {
        public int cx;
        public int cy;

        public FieldCell staticCell;
        public List<MovableCell> movableCells;

        public FieldCellSlot(int cx, int cy)
        {
            this.cx = cx;
            this.cy = cy;

            movableCells = new List<MovableCell>();
        }

        public void Reset()
        {
            staticCell = null;
            movableCells.Clear();
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
            return movableCells.Count + (staticCell != null ? 1 : 0);
        }

        public int MovableCount()
        {
            return movableCells.Count;
        }

        public bool ContainsSolid()
        {
            return GetSolid() != null;
        }

        public bool IsEmpty()
        {
            return staticCell == null && movableCells.Count == 0;
        }

        public SolidCell GetSolid()
        {
            return staticCell != null ? staticCell.AsSolid() : null;
        }

        public bool ContainsBrick()
        {
            return GetBrick() != null;
        }

        public BrickCell GetBrick()
        {
            return staticCell != null ? staticCell.AsBrick() : null;
        }

        public bool ContainsPowerup()
        {
            return GetPowerup() != null;
        }

        public PowerupCell GetPowerup()
        {
            return staticCell != null ? staticCell.AsPowerup() : null;
        }

        public bool ContainsFlame()
        {
            return GetFlame() != null;
        }

        public FlameCell GetFlame()
        {
            return staticCell != null ? staticCell.AsFlame() : null;
        }

        public bool ContainsBomb()
        {
            return GetBomb() != null;
        }

        public Bomb GetBomb()
        {
            for (int i = 0; i < movableCells.Count; ++i)
            {
                MovableCell cell = movableCells[i];
                if (cell.IsBomb())
                {
                    return cell.AsBomb();
                }
            }

            return null;
        }

        public bool ContainsPlayer()
        {
            for (int i = 0; i < movableCells.Count; ++i)
            {
                MovableCell cell = movableCells[i];
                if (cell.IsPlayer())
                {
                    return true;
                }
            }

            return false;
        }

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

            for (int i = 0; i < movableCells.Count; ++i)
            {
                MovableCell cell = movableCells[i];
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

            for (int i = 0; i < movableCells.Count; ++i)
            {
                list.AddLast(movableCells[i]);
            }
        }

        #endregion
    }
}

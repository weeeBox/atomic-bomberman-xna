using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Cells;
using BomberEngine;
using BomberEngine.Debugging;

namespace Bomberman.Game.Elements.Fields
{
    public class CellContactList
    {
        public LinkedList<MovableCell> cells;
        public MovableCell targetCell;

        public CellContactList(MovableCell targetCell)
        {
            this.targetCell = targetCell;
            cells = new LinkedList<MovableCell>();
        }

        public void Add(MovableCell cell)
        {
            Debug.Assert(!cells.Contains(cell));

            if (targetCell.IsMoving())
            {
                if (!cell.IsMoving() || cell.direction == targetCell.direction)
                {   
                    cells.AddFirst(cell);
                    return;
                }
            }

            cells.AddLast(cell);
        }

        public void Clear()
        {
            cells.Clear();
        }

        public int Size()
        {
            return cells.Count;
        }
    }
}

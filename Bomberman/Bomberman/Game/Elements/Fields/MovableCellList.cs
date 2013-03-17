using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Cells;
using BomberEngine.Debugging;

namespace Bomberman.Game.Elements.Fields
{
    public class MovableCellList
    {
        public LinkedList<MovableCell> list;

        public MovableCellList()
        {
            list = new LinkedList<MovableCell>();
        }

        public void Add(MovableCell cell)
        {
            Debug.Assert(Debug.flag && !list.Contains(cell));

            FieldCellType type = cell.type;
            for (LinkedListNode<MovableCell> node = list.Last; node != null; node = node.Previous)
            {
                FieldCellType otherCellType = node.Value.type;
                if (type > otherCellType)
                {
                    list.AddAfter(node, cell);
                    return;
                }
                if (type == otherCellType)
                {
                    list.AddAfter(node, cell);
                    return;
                }
            }
            list.AddFirst(cell);
        }

        public void Remove(MovableCell cell)
        {
            Debug.Assert(Debug.flag && list.Contains(cell));
            list.Remove(cell);
        }

        public int Size()
        {
            return list.Count;
        }
    }
}

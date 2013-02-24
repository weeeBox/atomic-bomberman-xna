using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Cells
{
    public class CellHelper
    {
        public static bool IsObstacleCell(FieldCell cell)
        {
            for (FieldCell c = cell; c != null; c = c.listNext)
            {
                if (c != null && c.IsObstacle())
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsCell(FieldCell root, FieldCellType type)
        {
            return FindCell(root, type) != null;
        }

        public static FieldCell FindCell(FieldCell root, FieldCellType type)
        {
            for (FieldCell c = root; c != null; c = c.listNext)
            {
                if (c.type == type)
                {
                    return c;
                }
            }
            return null;
        }

        public static int CellsCount(FieldCell root)
        {
            int count = 0;
            for (FieldCell c = root; c != null; c = c.listNext)
            {
                ++count;
            }
            return count;
        }

        public static void CopyToList(FieldCell root, List<FieldCell> list)
        {
            for (FieldCell c = root; c != null; c = c.listNext)
            {
                list.Add(c);
            }
        }
    }
}

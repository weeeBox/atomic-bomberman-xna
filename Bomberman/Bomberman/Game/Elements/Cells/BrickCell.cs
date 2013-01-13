using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Cells
{
    public class BrickCell : FieldCell
    {
        private bool solid;

        public BrickCell(int x, int y, bool solid)
            : base(x, y)
        {
            this.solid = solid;
        }

        public override bool IsBreakable()
        {
            return !solid;
        }

        public override bool IsSolid()
        {
            return solid;
        }
    }
}

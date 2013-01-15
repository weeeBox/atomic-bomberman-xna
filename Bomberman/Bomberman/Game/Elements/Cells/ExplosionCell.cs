using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Cells
{
    public class ExplosionCell : FieldCell
    {
        public ExplosionCell(int cx, int cy)
            : base(cx, cy)
        {
        }

        public override bool IsExplosion()
        {
            return true;
        }
    }
}

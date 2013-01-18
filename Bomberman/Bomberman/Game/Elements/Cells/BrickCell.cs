using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Cells
{
    public class BrickCell : FieldCell
    {
        public PowerupCell powerup;

        public BrickCell(int cx, int cy)
            : base(cx, cy)
        {
        }

        public override bool IsBrick()
        {
            return true;
        }

        public override bool IsObstacle()
        {
            return true;
        }
    }
}

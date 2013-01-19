using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Cells
{
    public class BrickCell : FieldCell
    {
        public int powerup;
        public bool destroyed;

        public BrickCell(int cx, int cy)
            : base(cx, cy)
        {
            powerup = Powerups.None;
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

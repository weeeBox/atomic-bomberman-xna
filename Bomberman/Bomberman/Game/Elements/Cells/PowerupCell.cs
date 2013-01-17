using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Cells
{
    public class PowerupCell : FieldCell
    {
        public Powerups powerup;

        public PowerupCell(Powerups powerup, int cx, int cy)
            : base(cx, cy)
        {
            this.powerup = powerup;
        }

        public override bool IsPowerup()
        {
            return true;
        }
    }
}

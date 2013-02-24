using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Cells
{
    public class PowerupCell : FieldCell
    {
        public int powerup;
        public float elasped;

        public PowerupCell(int
            powerup, int cx, int cy)
            : base(FieldCellType.Powerup, cx, cy)
        {
            this.powerup = powerup;
        }

        public override void Update(float delta)
        {
            elasped += delta;
        }

        public override PowerupCell AsPowerup()
        {
            return this;
        }

        public override bool IsPowerup()
        {
            return true;
        }
    }
}

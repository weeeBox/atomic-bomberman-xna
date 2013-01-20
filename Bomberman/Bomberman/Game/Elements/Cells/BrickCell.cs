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
        public float remains;

        public BrickCell(int cx, int cy)
            : base(cx, cy)
        {
            powerup = Powerups.None;
        }

        public void Destroy()
        {
            if (!destroyed)
            {
                destroyed = true;
                remains = 0.5f;
            }
        }

        public override void Update(float delta)
        {
            if (destroyed)
            {
                remains -= delta;
                if (remains <= 0)
                {
                    GetField().DestroyBrick(this);
                }
            }
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

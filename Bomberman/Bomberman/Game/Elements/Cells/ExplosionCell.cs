using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Cells
{
    public class ExplosionCell : FieldCell
    {
        private static readonly float TIMEOUT = 0.5f;

        private float remains;

        public ExplosionCell(int cx, int cy)
            : base(cx, cy)
        {
            remains = TIMEOUT;
        }

        public override void Update(float delta)
        {
            remains -= delta;
            if (remains <= 0)
            {
                GetField().ClearCell(cx, cy);
            }
        }

        public override bool IsExplosion()
        {
            return true;
        }
    }
}

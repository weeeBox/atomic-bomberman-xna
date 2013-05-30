using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements.Players;

namespace Bomberman.Game.Elements.Cells
{
    public class FlameCell : FieldCell
    {
        private float remains;

        public Player player;

        public FlameCell(Player player, int cx, int cy)
            : base(FieldCellType.Flame, cx, cy)
        {
            this.player = player;
            remains = CVars.cg_timeFlame.intValue * 0.001f;
        }

        public override void Update(float delta)
        {
            remains -= delta;
            if (remains <= 0)
            {
                GetField().RemoveCell(this);
            }
        }

        public override bool IsFlame()
        {
            return true;
        }

        public override FlameCell AsFlame()
        {
            return this;
        }
    }
}

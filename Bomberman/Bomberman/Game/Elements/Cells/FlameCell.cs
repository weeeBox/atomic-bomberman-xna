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
        private float m_remains;
        private Player m_player;

        public FlameCell(Player player, int cx, int cy)
            : base(FieldCellType.Flame, cx, cy)
        {
            m_player = player;
            m_remains = CVars.cg_timeFlame.intValue * 0.001f;
        }

        public override void Update(float delta)
        {
            m_remains -= delta;
            if (m_remains <= 0)
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

        public float remains
        {
            get { return m_remains; }
        }

        public Player player
        {
            get { return m_player; }
        }
    }
}

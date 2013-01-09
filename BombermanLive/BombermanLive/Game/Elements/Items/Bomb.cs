using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BombermanLive.Game.Elements.Players;

namespace BombermanLive.Game.Elements.Items
{
    public class Bomb : FieldCell
    {
        private Player player;
        private int explosionLength;
        private float explosionTimeout;

        public Bomb(int x, int y) : base(x, y)
        {
        }

        public void Reset()
        {
            player = null;
        }
    }
}

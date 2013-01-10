using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Cells;
using BomberEngine.Util;

namespace Bomberman.Game.Elements.Items
{
    public class Bomb : MovingCell
    {
        private Player player;
        
        private int radius;
        private float timeout;
        private bool dud;
        private bool bouncing;
        private bool detonated;

        public Bomb(Player player, bool dud) : base(player.GetX(), player.GetY())
        {
            this.player = player;
            this.radius = player.GetBombRadius();
            this.timeout = player.GetBombTimeout();
            this.dud = dud;
            this.bouncing = player.IsBombBouncing();
            this.detonated = player.IsBobmDetonated();
        }

        public Player GetPlayer()
        {
            return player;
        }

        public int GetRadius()
        {
            return radius;
        }

        public float GetTimeout()
        {
            return timeout;
        }

        public bool IsDud()
        {
            return dud;
        }

        public bool IsBouncing()
        {
            return bouncing;
        }

        public bool IsDetonated()
        {
            return detonated;
        }
    }
}

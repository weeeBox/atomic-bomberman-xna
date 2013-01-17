using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Cells;
using BomberEngine.Util;
using BomberEngine.Core;

namespace Bomberman.Game.Elements.Cells
{
    public class Bomb : MovableCell
    {
        private Player player;
        
        private int radius;
        private float remains;
        private bool dud;
        private bool bouncing;
        private bool detonated;
        private bool exploded;

        public Bomb(Player player, bool dud) : base(player.GetCx(), player.GetCy())
        {
            this.player = player;
            this.radius = player.GetBombRadius();
            remains = player.GetBombTimeout();
            this.dud = dud;
            this.bouncing = player.IsBombBouncing();
            this.detonated = player.IsBobmDetonated();
        }

        public override void Update(float delta)
        {
            remains -= delta;
            if (remains <= 0)
            {
                GetField().BlowBomb(this);
            }
        }

        public Player GetPlayer()
        {
            return player;
        }

        public override bool IsBomb()
        {
            return true;
        }

        public override bool IsObstacle()
        {
            return true;
        }

        public int GetRadius()
        {
            return radius;
        }

        public float GetTimeout()
        {
            return remains;
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

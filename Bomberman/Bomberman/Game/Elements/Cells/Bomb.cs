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
        public bool active;
        
        private int radius;
        private float remains;
        private bool dud;
        private bool bouncing;
        private bool detonated;

        public Bomb(Player player) : base(player.GetCx(), player.GetCy())
        {
            this.player = player;
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

        public void Activate()
        {
            active = true;
            SetCell(player.GetCx(), player.GetCy());
            radius = player.GetBombRadius();
            remains = player.GetBombTimeout();
            bouncing = player.IsBombBouncing();
            detonated = player.IsBobmDetonated();
            dud = false;
        }

        public void Blow()
        {
            active = false;
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

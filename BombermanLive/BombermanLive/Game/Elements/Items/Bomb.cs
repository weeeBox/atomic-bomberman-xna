using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BombermanLive.Game.Elements.Players;
using BombermanLive.Game.Elements.Cells;

namespace BombermanLive.Game.Elements.Items
{
    public class Bomb : MovingCell
    {
        private Player player;
        private int radius;
        private float timeout;
        private bool dud;
        private bool bouncing;
        private bool detonated;

        public Bomb(int x, int y) : base(x, y)
        {
            Reset();
        }

        public void Reset()
        {
            player = null;
            radius = 1;
            dud = false;
            bouncing = false;
            detonated = false;
        }

        public Player GetPlayer()
        {
            return player;
        }

        public void SetPlayer(Player player)
        {
            this.player = player;
        }

        public int GetRadius()
        {
            return radius;
        }

        public void SetRadius(int radius)
        {
            this.radius = radius;
        }

        public float GetTimeout()
        {
            return timeout;
        }

        public void SetTimeout(float timeout)
        {
            this.timeout = timeout;
        }

        public bool IsDud()
        {
            return dud;
        }

        public void SetDud(bool dud)
        {
            this.dud = dud;
        }

        public bool IsBouncing()
        {
            return bouncing;
        }

        public void SetBouncing(bool bouncing)
        {
            this.bouncing = bouncing;
        }

        public bool IsDetonated()
        {
            return detonated;
        }

        public void SetDetonated(bool detonated)
        {
            this.detonated = detonated;
        }
    }
}

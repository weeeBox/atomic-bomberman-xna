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
        public float remains;
        private bool dud;
        private bool jelly;
        private bool trigger;

        public Bomb(Player player) : base(player.GetCx(), player.GetCy())
        {
            this.player = player;
            SetSpeed(Settings.Get(Settings.VAL_BOMB_ROLL_SPEED));
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (!trigger)
            {
                remains -= delta;
                if (remains <= 0)
                {
                    GetField().BlowBomb(this);
                }
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
            jelly = player.IsJelly();
            trigger = player.IsTrigger();
            dud = false;
        }

        public void Blow()
        {
            SetCell();
            StopMoving();
            active = false;
        }

        public void Kick(Direction direction)
        {
            SetMoveDirection(direction);
        }

        public override Bomb AsBomb()
        {
            return this;
        }

        public override bool IsBomb()
        {
            return true;
        }

        public override bool IsObstacle()
        {
            return true;
        }

        public override void HitObstacle(Fields.FieldCell obstacle)
        {
            base.HitObstacle(obstacle);
            TryJellyOnObstacle();
        }

        public override void HitWall()
        {
            base.HitWall();
            TryJellyOnObstacle();
        }

        private void TryJellyOnObstacle()
        {
            if (IsJelly())
            {
                SetMoveDirection(Util.Opposite(direction));
            }
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

        public bool IsJelly()
        {
            return jelly;
        }

        public bool IsTrigger()
        {
            return trigger;
        }
    }
}

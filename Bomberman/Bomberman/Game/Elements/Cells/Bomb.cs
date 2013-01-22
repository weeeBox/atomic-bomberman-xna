using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Cells;
using BomberEngine.Util;
using BomberEngine.Core;
using BomberEngine;
using BomberEngine.Debugging;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Cells
{
    public class Bomb : MovableCell
    {
        private static int nextTriggerIndex;

        private Player player;
        public bool active;
        
        private int radius;
        public float remains;
        private bool dud;
        private bool jelly;

        private bool m_trigger;
        private bool m_grabbed;

        private int m_triggerIndex;

        public Bomb(Player player) : base(player.GetCx(), player.GetCy())
        {
            this.player = player;
            SetSpeed(Settings.Get(Settings.VAL_BOMB_ROLL_SPEED));
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (IsUpdatable())
            {
                remains -= delta;
                if (remains <= 0)
                {
                    Blow();
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
            m_trigger = player.IsTrigger();
            if (m_trigger)
            {
                m_triggerIndex = nextTriggerIndex++;
            }
            
            dud = false;
        }

        public void Blow()
        {
            SetCell();
            StopMoving();
            active = false;

            GetField().BlowBomb(this);
        }

        public void Kick(Direction direction)
        {
            SetMoveDirection(direction);
        }

        public void Grab()
        {
            m_grabbed = true;
            GetField().ClearCell(cx, cy);
        }

        public void Throw(Direction direction, int fromCx, int fromCy)
        {
            int mulX = 0;
            int mulY = 0;

            switch (direction)
            {
                case Direction.DOWN:
                    {
                        mulY = 1;
                        break;
                    }
                case Direction.UP:
                    {
                        mulY = -1;
                        break;
                    }
                case Direction.LEFT:
                    {
                        mulX = -1;
                        break;
                    }
                case Direction.RIGHT:
                    {
                        mulX = 1;
                        break;
                    }
                default:
                    Debug.Assert(false, "Unexpected direction: " + direction);
                    break;

            }

            int throwDistance = Settings.Get(Settings.VAL_BOMB_THROW_DISTANCE);
            int destCx = fromCx + mulX * throwDistance;
            int destCy = fromCy + mulY * throwDistance;

            Field field = GetField();
            if (field.IsObstacleCell(destCx, destCy))
            {

            }
            else
            {
                SetCell(destCx, destCy);
                field.SetBomb(this);
            }

            m_grabbed = false;
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
            return m_trigger;
        }

        private bool IsUpdatable()
        {
            return !m_trigger && !m_grabbed;
        }

        public bool trigger
        {
            get { return m_trigger; }
        }

        public int triggerIndex
        {
            get { return m_triggerIndex; }
        }
    }
}

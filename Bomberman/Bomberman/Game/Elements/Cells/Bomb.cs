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
        private const byte STATE_NORMAL = 0;
        private const byte STATE_GRABBED = 1;
        private const byte STATE_THROWN = 2;
        private const byte STATE_PUNCHED = 3;
        private const byte STATE_JUMPING = 4;

        private static int nextTriggerIndex;

        private Player player;
        private UpdatableDelegate updater;

        public bool active;

        private float flySpeed;
        private float flyDistance;

        private float fallSpeed;
        public float fallHeight;
        private float fallGravity;

        private int radius;
        public float remains;

        private bool dud;
        private bool jelly;

        private bool m_trigger;
        private byte m_state;

        private int m_triggerIndex;

        public Bomb(Player player) : base(player.GetCx(), player.GetCy())
        {
            this.player = player;
            SetSpeed(Settings.Get(Settings.VAL_BOMB_ROLL_SPEED));
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            updater(delta);            
        }

        private void UpdateNormal(float delta)
        {
            if (!IsTrigger())
            {
                remains -= delta;
                if (remains <= 0)
                {
                    Blow();
                }
            }
        }

        private void UpdateGrabbed(float delta)
        {
        }

        private void UpdateFlying(float delta)
        {
            float shift = flyDistance;

            switch (direction)
            {
                case Direction.UP:
                {
                    shift = Math.Max(flySpeed * delta, shift);
                    MoveY(shift);
                    break;
                }

                case Direction.DOWN:
                {
                    shift = Math.Min(flySpeed * delta, shift);
                    MoveY(shift);
                    break;
                }

                case Direction.LEFT:
                {
                    shift = Math.Max(flySpeed * delta, shift);
                    MoveX(shift);
                    break;
                }

                case Direction.RIGHT:
                {
                    shift = Math.Min(flySpeed * delta, shift);
                    MoveX(shift);
                    break;
                }
            }

            fallHeight += fallSpeed * delta;
            fallSpeed -= fallGravity * delta;

            flyDistance -= shift;
            if (flyDistance == 0)
            {
                fallHeight = 0;
                EndFlying();
            }
        }

        private void UpdateJumping(float delta)
        {

        }

        protected override void OnPositionChanged(float oldPx, float oldPy)
        {
            if (IsNormal())
            {
                base.OnPositionChanged(oldPx, oldPy);
            }
        }

        protected override void OnCellChanged(int oldCx, int oldCy)
        {
            if (IsNormal())
            {
                base.OnCellChanged(oldCx, oldCy);
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
            SetState(STATE_NORMAL);
            dud = false;
            jelly = player.IsJelly();
            m_trigger = player.IsTrigger();
            if (m_trigger)
            {
                m_triggerIndex = nextTriggerIndex++;
            }
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
            SetState(STATE_GRABBED);
            RemoveFromField();
        }

        public void Throw()
        {
            float fromPx = player.px;
            float fromPy = player.py;
            Direction direction = player.direction;

            SetPixels(fromPx, fromPy);

            Fly(fromPx, fromPy, direction);
            SetState(STATE_THROWN);
        }

        private void Fly(float fromPx, float fromPy, Direction direction)
        {
            Fly(fromPx, fromPy, direction, Settings.Get(Settings.VAL_BOMB_FLY_DISTANCE));
        }

        private void Jump(float fromPx, float fromPy, Direction direction)
        {
            Fly(fromPx, fromPy, direction, Settings.Get(Settings.VAL_BOMB_JUMP_DISTANCE));
        }

        private void Fly(float fromPx, float fromPy, Direction direction, int numCells)
        {
            int fromCx = Util.Px2Cx(fromPx);
            int fromCy = Util.Py2Cy(fromPy);

            flySpeed = Settings.Get(Settings.VAL_BOMB_FLY_SPEED);

            switch (direction)
            {
                case Direction.LEFT:
                    flySpeed = -flySpeed;
                    flyDistance = Util.TravelDistanceX(fromPx, fromCx - numCells);
                    break;

                case Direction.RIGHT:
                    flyDistance = Util.TravelDistanceX(fromPx, fromCx + numCells);
                    break;

                case Direction.UP:
                    flySpeed = -flySpeed;
                    flyDistance = Util.TravelDistanceY(fromPy, fromCy - numCells);
                    break;

                case Direction.DOWN:
                    flyDistance = Util.TravelDistanceY(fromPy, fromCy + numCells);
                    break;
            }

            fallHeight = 0;
            fallGravity = Settings.Get(Settings.VAL_BOMB_DROP_GRAVITY);
            fallSpeed = 0.5f * fallGravity * flyDistance / flySpeed;

            SetDirection(direction);
        }

        private void EndFlying()
        {
            Field field = GetField();
            if (field.IsObstacleCell(cx, cy))
            {
                Jump(px, py, direction);
                SetState(STATE_JUMPING);
            }
            else
            {
                if (IsThrown())
                {
                    remains = player.GetBombTimeout();
                }
                
                SetState(STATE_NORMAL);
                player.BombLanded(this);
            }
        }

        public void Punch()
        {
            RemoveFromField();
            Fly(px, py, player.direction);
            SetState(STATE_PUNCHED);
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

        private void SetState(byte state)
        {
            switch (state)
            {
                case STATE_NORMAL:
                    updater = UpdateNormal;
                    break;

                case STATE_GRABBED:
                    Debug.Assert(m_state == STATE_NORMAL);
                    updater = UpdateGrabbed;
                    break;

                case STATE_THROWN:
                    Debug.Assert(m_state == STATE_GRABBED);
                    updater = UpdateFlying;
                    break;

                case STATE_PUNCHED:
                    Debug.Assert(m_state == STATE_NORMAL);
                    updater = UpdateFlying;
                    break;

                case STATE_JUMPING:
                    Debug.Assert(m_state == STATE_JUMPING || m_state == STATE_THROWN || m_state == STATE_PUNCHED);
                    updater = UpdateFlying;
                    break;
            }

            m_state = state;
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

        public bool CanTrigger()
        {
            return IsTrigger() && IsNormal();
        }

        public bool IsNormal()
        {
            return m_state == STATE_NORMAL;
        }

        public bool IsGrabbed()
        {
            return m_state == STATE_GRABBED;
        }

        public bool IsThrown()
        {
            return m_state == STATE_THROWN;
        }

        public bool IsJumping()
        {
            return m_state == STATE_JUMPING;
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

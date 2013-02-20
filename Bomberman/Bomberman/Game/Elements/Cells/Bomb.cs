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
        private const byte STATE_FLYING = 2;

        private static int nextTriggerIndex;

        private Player m_player;
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
            m_player = player;
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

        protected override void OnPositionChanged()
        {
            if (IsNormal())
            {
                base.OnPositionChanged();
            }
            else if (IsFlying())
            {
                NormalizeFlyingPos();
            }
        }

        private void NormalizeFlyingPos()
        {
            float dx = moveDx;
            float dy = moveDy;

            if (dx > 0) // moving right
            {
                if (px > GetField().GetMaxPx() + Constant.CELL_WIDTH_2)
                {
                    SetPosX(GetField().GetMinPx() - Constant.CELL_WIDTH_2);
                }
            }
            else if (dx < 0) // moving left
            {
                if (px < GetField().GetMinPx() - Constant.CELL_WIDTH_2)
                {
                    SetPosX(GetField().GetMaxPx() + Constant.CELL_WIDTH_2);
                }
            }

            if (dy > 0) // moving down
            {
                if (py > GetField().GetMaxPy() + Constant.CELL_HEIGHT_2)
                {
                    SetPosY(GetField().GetMinPy() - Constant.CELL_HEIGHT_2);
                }
            }
            else if (dy < 0) // moving up
            {
                if (py < GetField().GetMinPy() - Constant.CELL_HEIGHT_2)
                {
                    SetPosY(GetField().GetMaxPy() + Constant.CELL_HEIGHT_2);
                }
            }
        }

        protected override void OnCellChanged(int oldCx, int oldCy)
        {
            if (IsNormal())
            {
                base.OnCellChanged(oldCx, oldCy);
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Collisions

        public override bool HandleCollision(FieldCell cell)
        {
            if (cell.IsObstacle())
            {
                Bomb other = cell.AsBomb();
                if (other != null && other.moving)
                {
                    other.HandleObstacleCollision(cell);
                }

                return HandleObstacleCollision(cell);
            }
            if (cell.IsPowerup())
            {
                return HandleCollision(cell.AsPowerup());
            }
            if (cell.IsFlame())
            {
                return HandleCollision(cell.AsFlame());
            }

            return base.HandleCollision(cell);
        }

        public override bool HandleWallCollision()
        {
            if (TryJellyOnObstacle())
            {
                return true;
            }
            return base.HandleWallCollision();
        }

        protected override bool HandleObstacleCollision(FieldCell cell)
        {
            base.HandleObstacleCollision(cell);
            StopMoving();
            TryJellyOnObstacle();

            return false;
        }

        private bool HandleCollision(FlameCell flame)
        {
            Blow();
            return true;
        }

        private bool HandleCollision(PowerupCell powerupCell)
        {   
            GetField().ClearCell(powerupCell.cx, powerupCell.cy);
            return true;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public void Activate()
        {
            active = true;
            SetCell(player.cx, player.cy);
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
            float fromPx = m_player.px;
            float fromPy = m_player.py;
            Direction direction = m_player.direction;

            SetPixels(fromPx, fromPy);

            Fly(fromPx, fromPy, direction);
            remains = m_player.GetBombTimeout();
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
            SetState(STATE_FLYING);
        }

        private void EndFlying()
        {
            Field field = GetField();
            if (field.IsObstacleCell(cx, cy))
            {
                Jump(px, py, direction);
            }
            else
            {   
                SetState(STATE_NORMAL);
                m_player.OnBombLanded(this);
            }
        }

        public void Punch()
        {
            RemoveFromField();
            Fly(px, py, m_player.direction);
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

        private bool TryJellyOnObstacle()
        {
            if (IsJelly())
            {
                SetMoveDirection(Util.Opposite(direction));
                return true;
            }

            return false;
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

                case STATE_FLYING:
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

        public bool IsFlying()
        {
            return m_state == STATE_FLYING;
        }

        public Player player
        {
            get { return m_player; }
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

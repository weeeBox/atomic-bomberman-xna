﻿using System;
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
        private enum State
        {
            Undefined,
            Normal,
            Grabbed,
            Flying
        }

        private static int s_nextTriggerIndex;

        private Player m_player;
        private UpdatableDelegate m_updater;

        private bool m_active;

        private float m_flySpeed;
        private float m_flyDistance;

        private float m_fallSpeed;
        private float m_fallHeight;
        private float m_fallGravity;

        private int m_radius;
        private float m_timeRemains;

        private bool m_dudFlag;
        private bool m_jelly;

        private bool m_trigger;
        private State m_state;

        private int m_triggerIndex;

        private bool m_jellyBounced;

        public Bomb(Player player)
           : base(FieldCellType.Bomb, player.GetCx(), player.GetCy())
        {
            m_player = player;
            m_state = State.Undefined;
            SetSpeed(CVars.cg_bombRollSpeed.intValue);
        }

        public override void Reset()
        {
            base.Reset();

            m_updater = null;
            m_active = false;
            m_flySpeed = 0.0f;
            m_flyDistance = 0.0f;
            m_fallSpeed = 0.0f;
            m_fallHeight = 0.0f;
            m_fallGravity = 0.0f;
            m_radius = 0;
            m_timeRemains = 0;
            m_dudFlag = false;
            m_jelly = false;
            m_trigger = false;
            m_state = State.Undefined;
            m_triggerIndex = 0;
            m_jellyBounced = false;

            SetSpeed(CVars.cg_bombRollSpeed.intValue);
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            m_updater(delta);            
        }

        private void UpdateNormal(float delta)
        {
            if (!IsTrigger())
            {
                m_timeRemains -= delta;
                if (m_timeRemains <= 0)
                {
                    Blow();
                }
            }

            if (IsJelly())
            {
                m_jellyBounced = false;
            }
        }

        private void UpdateGrabbed(float delta)
        {
        }

        private void UpdateFlying(float delta)
        {
            float shift = m_flyDistance;

            switch (direction)
            {
                case Direction.UP:
                {
                    shift = Math.Max(m_flySpeed * delta, shift);
                    MoveY(shift);
                    break;
                }

                case Direction.DOWN:
                {
                    shift = Math.Min(m_flySpeed * delta, shift);
                    MoveY(shift);
                    break;
                }

                case Direction.LEFT:
                {
                    shift = Math.Max(m_flySpeed * delta, shift);
                    MoveX(shift);
                    break;
                }

                case Direction.RIGHT:
                {
                    shift = Math.Min(m_flySpeed * delta, shift);
                    MoveX(shift);
                    break;
                }
            }

            m_fallHeight += m_fallSpeed * delta;
            m_fallSpeed -= m_fallGravity * delta;

            m_flyDistance -= shift;
            if (m_flyDistance == 0)
            {
                m_fallHeight = 0;
                EndFlying();
            }
        }

        protected override void OnCellMoved()
        {
            if (IsFlying())
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
                if (cell.IsBomb())
                {
                    return HandleCollision(cell.AsBomb());
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
            if (cell.IsPlayer())
            {
                return HandleCollision(cell.AsPlayer());
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

        public override bool HandleObstacleCollision(FieldCell cell)
        {
            MoveOutOfCollision(this, cell);
            if (!TryJellyOnObstacle())
            {
                StopMoving();
            }

            return true;
        }

        private bool HandleCollision(Bomb bomb)
        {
            if (bomb.IsJelly())
            {
                bomb.HandleObstacleCollision(this);
            }
            return HandleObstacleCollision(bomb);
        }

        private bool HandleCollision(FlameCell flame)
        {
            Blow();
            return true;
        }

        private bool HandleCollision(PowerupCell powerupCell)
        {
            GetField().RemoveCell(powerupCell);
            return true;
        }

        private bool HandleCollision(Player player)
        {
            return player.HandleCollision(this);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public void Activate()
        {   
            m_active = true;
            SetCell(player.cx, player.cy);
            m_radius = player.GetBombRadius();
            m_timeRemains = player.GetBombTimeout();
            SetState(State.Normal);
            m_dudFlag = false;
            m_jelly = player.IsJelly();
            m_trigger = player.IsTrigger();
            if (m_trigger)
            {
                m_triggerIndex = s_nextTriggerIndex++;
            }
        }

        public void Blow()
        {
            SetCell();
            StopMoving();
            m_active = false;

            GetField().BlowBomb(this);
        }

        public void Kick(Direction direction)
        {
            SetMoveDirection(direction);
        }

        public void Grab()
        {
            SetState(State.Grabbed);
            RemoveFromField();
        }

        public void Throw()
        {
            float fromPx = m_player.px;
            float fromPy = m_player.py;
            Direction direction = m_player.direction;

            SetPos(fromPx, fromPy);

            Fly(fromPx, fromPy, direction);
            m_timeRemains = m_player.GetBombTimeout();
        }

        private void Fly(float fromPx, float fromPy, Direction direction)
        {
            Fly(fromPx, fromPy, direction, CVars.cg_bombFlyDistance.intValue);
        }

        private void Jump(float fromPx, float fromPy, Direction direction)
        {
            Fly(fromPx, fromPy, direction, CVars.cg_bombJumpDistance.intValue);
        }

        private void Fly(float fromPx, float fromPy, Direction direction, int numCells)
        {
            int fromCx = Util.Px2Cx(fromPx);
            int fromCy = Util.Py2Cy(fromPy);

            m_flySpeed = CVars.cg_bombFlySpeed.intValue;

            switch (direction)
            {
                case Direction.LEFT:
                    m_flySpeed = -m_flySpeed;
                    m_flyDistance = Util.TravelDistanceX(fromPx, fromCx - numCells);
                    break;

                case Direction.RIGHT:
                    m_flyDistance = Util.TravelDistanceX(fromPx, fromCx + numCells);
                    break;

                case Direction.UP:
                    m_flySpeed = -m_flySpeed;
                    m_flyDistance = Util.TravelDistanceY(fromPy, fromCy - numCells);
                    break;

                case Direction.DOWN:
                    m_flyDistance = Util.TravelDistanceY(fromPy, fromCy + numCells);
                    break;
            }

            m_fallHeight = 0;
            m_fallGravity = CVars.cg_bombDropGravity.intValue;
            m_fallSpeed = 0.5f * m_fallGravity * m_flyDistance / m_flySpeed;

            SetDirection(direction);
            SetState(State.Flying);
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
                SetState(State.Normal);
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
            if (m_jellyBounced)
            {
                return true;
            }

            if (IsJelly())
            {   
                m_jellyBounced = true;
                SetMoveDirection(Util.Opposite(direction));
                return true;
            }

            return false;
        }

        private void SetState(State state)
        {
            switch (state)
            {
                case State.Normal:
                    m_updater = UpdateNormal;
                    break;

                case State.Grabbed:
                    Debug.Assert(m_state == State.Normal);
                    m_updater = UpdateGrabbed;
                    break;

                case State.Flying:
                    m_updater = UpdateFlying;
                    break;
            }

            m_state = state;
        }

        public int GetRadius()
        {
            return m_radius;
        }

        public bool IsDud()
        {
            return m_dudFlag;
        }

        public bool IsJelly()
        {
            return m_jelly;
        }

        public void SetJelly(bool jelly)
        {
            m_jelly = jelly;
        }

        public bool IsTrigger()
        {
            return m_trigger;
        }

        public void SetTrigger(bool trigger)
        {
            m_trigger = trigger;
        }

        public bool CanTrigger()
        {
            return IsTrigger() && IsNormal();
        }

        public bool IsNormal()
        {
            return m_state == State.Normal;
        }

        public bool IsGrabbed()
        {
            return m_state == State.Grabbed;
        }

        public bool IsFlying()
        {
            return m_state == State.Flying;
        }

        public Player player
        {
            get { return m_player; }
            set { m_player = value; }
        }

        public bool trigger
        {
            get { return m_trigger; }
        }

        public int triggerIndex
        {
            get { return m_triggerIndex; }
        }

        public bool active
        {
            get { return m_active; }
            set { m_active = value; }
        }

        public float timeRemains
        {
            get { return m_timeRemains; }
            set { m_timeRemains = value; }
        }

        public float fallHeight
        {
            get { return m_fallHeight; }
        }

        public bool jellyBounced
        {
            get { return m_jellyBounced; }
        }
    }
}

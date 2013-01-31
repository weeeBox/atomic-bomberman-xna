using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;
using BomberEngine.Util;

namespace Bomberman.Game.Elements.Cells
{
    public class MovableCell : FieldCell
    {
        private Direction m_direction;
        private Direction m_oldDirection;

        /* Points per second */
        private float m_speed;

        /* from 0..1 */
        private float m_moveKx;
        private float m_moveKy;

        private bool m_moving;

        public MovableCell(int cx, int cy)
            : base(cx, cy)
        {
            m_direction = Direction.DOWN;
            m_oldDirection = Direction.DOWN;
        }

        public override void Update(float delta)
        {
            if (m_moving)
            {
                UpdateMoving(delta);
            }
        }

        protected virtual void UpdateMoving(float delta)
        {   
            float offset = m_speed * delta;
            float dx = m_moveKx * offset;
            float dy = m_moveKy * offset;

            moveDx = dx;
            moveDy = dy;

            Move(dx, dy);

            moveDx = 0;
            moveDy = 0;
        }

        protected virtual void MoveToTargetPx(float delta)
        {
            float xOffset = Util.TargetPxOffset(px);
            MoveX(xOffset < 0 ? Math.Max(xOffset, -delta * m_speed) : Math.Min(xOffset, delta * m_speed));
        }

        protected virtual void MoveToTargetPy(float delta)
        {
            float yOffset = Util.TargetPyOffset(py);
            MoveY(yOffset < 0 ? Math.Max(yOffset, -delta * m_speed) : Math.Min(yOffset, delta * m_speed));
        }

        public virtual void OnHitWall()
        {
            StopMoving();
        }

        public virtual void OnHitObstacle(FieldCell obstacle)
        {
            StopMoving();
        }

        protected void SetMoveDirection(Direction direction)
        {
            SetDirection(direction);
            m_moving = true;

            switch (direction)
            {
                case Direction.UP:
                    m_moveKy = -1.0f;
                    break;
                case Direction.DOWN:
                    m_moveKy = 1.0f;
                    break;
                case Direction.LEFT:
                    m_moveKx = -1.0f;
                    break;
                case Direction.RIGHT:
                    m_moveKx = 1.0f;
                    break;
            }
        }

        public void StopMoving()
        {
            m_moving = false;
            m_moveKx = 0.0f;
            m_moveKy = 0.0f;
        }

        public void SetDirection(Direction newDirection)
        {   
            m_oldDirection = m_direction;
            m_direction = newDirection;
        }

        public void MoveBackX(float distance)
        {
            SetPosX(px - MathHelper.Sign(moveDx) * distance);
        }

        public void MoveBackY(float distance)
        {
            SetPosY(py - MathHelper.Sign(moveDy) * distance);
        }

        public void MoveX(float dx)
        {
            Move(dx, 0);
        }

        public void MoveY(float dy)
        {
            Move(0, dy);
        }

        public void Move(float dx, float dy)
        {
            if (dx != 0.0f || dy != 0.0f)
            {   
                // update pixel cords without updating cell cords
                SetPixelCords(px + dx, py + dy);

                // check collisions and all the stuff
                OnPositionChanged();

                int oldCx = cx;
                int oldCy = cy;

                // update cell cords
                SetCellCords();

                if (cx != oldCx || cy != oldCy)
                {
                    OnCellChanged(oldCx, oldCy);
                }
            }
        }

        public void SetPosX(float px)
        {
            SetPos(px, py);
        }

        public void SetPosY(float py)
        {
            SetPos(px, py);
        }

        public void SetPos(float px, float py)
        {
            SetPixelCords(px, py);
            SetCellCords();
        }

        protected virtual void OnPositionChanged()
        {
            GetField().MoveablePosChanged(this);
        }

        protected virtual void OnCellChanged(int oldCx, int oldCy)
        {
            GetField().MovableCellChanged(this, oldCx, oldCy);
        }

        public void SetSpeed(float speed)
        {
            this.m_speed = speed;
        }

        public void IncSpeed(float amount)
        {
            SetSpeed(m_speed + amount);
        }

        public float GetSpeed()
        {
            return m_speed;
        }

        public override bool IsMovable()
        {
            return true;
        }

        public override MovableCell AsMovable()
        {
            return this;
        }

        public bool moving
        {
            get { return m_moving; }
        }

        public Direction direction
        {
            get { return m_direction; }
        }

        public Direction oldDirection
        {
            get { return m_oldDirection; }
        }
    }
}

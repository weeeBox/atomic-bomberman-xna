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

        /* from -1..1 */
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
            
            switch (direction)
            {
                case Direction.LEFT:
                case Direction.RIGHT:
                    {
                        float oldPx = px;
                        float dx = m_moveKx * offset;

                        MoveX(dx);

                        if (oldPx == px) // blocking horizontal movement
                        {
                            int step = Math.Sign(dx);

                            bool cameAroundObstacle = TryComeRoundObstacleX(delta, step);
                            if (!cameAroundObstacle)
                            {
                                MoveToTargetPy(delta);
                            }
                        }
                        else
                        {
                            MoveToTargetPy(delta);
                        }

                        break;
                    }

                case Direction.UP:
                case Direction.DOWN:
                    {
                        float oldPy = py;
                        float dy = m_moveKy * offset;
                        MoveY(dy);

                        if (py == oldPy) // blocking vertical movement
                        {
                            int step = Math.Sign(dy);

                            bool cameAroundObstacle = TryComeRoundObstacleY(delta, step);
                            if (!cameAroundObstacle)
                            {
                                MoveToTargetPx(delta);
                            }
                        }
                        else
                        {
                            MoveToTargetPx(delta);
                        }

                        break;
                    }
            }
        }

        private bool TryComeRoundObstacleX(float delta, int step)
        {
            FieldCell blockingCell = NearCell(step, 0);
            if (blockingCell != null)
            {
                if (blockingCell.IsObstacle())
                {
                    float blockingPy = blockingCell.GetPy();
                    if (py < blockingPy && !GetField().IsObstacleCell(cx + step, cy - 1))
                    {
                        MoveY(Math.Max(Util.Cy2Py(cy - 1) - py, -m_speed * delta));
                        return true;
                    }
                    if (py > blockingPy && !GetField().IsObstacleCell(cx + step, cy + 1))
                    {
                        MoveY(Math.Min(Util.Cy2Py(cy + 1) - py, m_speed * delta));
                        return true;
                    }
                }
            }

            return false;
        }

        private bool TryComeRoundObstacleY(float delta, int step)
        {
            FieldCell blockingCell = NearCell(0, step);
            if (blockingCell != null)
            {
                if (blockingCell.IsObstacle())
                {
                    float blockingPx = blockingCell.GetPx();
                    if (px < blockingPx && !GetField().IsObstacleCell(cx - 1, cy + step))
                    {
                        MoveX(Math.Max(Util.Cx2Px(cx - 1) - px, -m_speed * delta));
                        return true;
                    }
                    if (px > blockingPx && !GetField().IsObstacleCell(cx + 1, cy + step))
                    {
                        MoveX(Math.Min(Util.Cx2Px(cx + 1) - px, m_speed * delta));
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual void MoveToTargetPx(float delta)
        {
            float xOffset = Util.TargetPxOffset(px, m_oldDirection);
            MoveX(xOffset < 0 ? Math.Max(xOffset, -delta * m_speed) : Math.Min(xOffset, delta * m_speed));
        }

        protected virtual void MoveToTargetPy(float delta)
        {
            float yOffset = Util.TargetPyOffset(py, m_oldDirection);
            MoveY(yOffset < 0 ? Math.Max(yOffset, -delta * m_speed) : Math.Min(yOffset, delta * m_speed));
        }

        public virtual bool HandleWallCollision()
        {
            StopMoving();
            return true;
        }

        public virtual bool HandleCollision(FieldCell cell)
        {
            return false;
        }

        protected virtual bool HandleObstacleCollision(FieldCell cell)
        {
            return MoveOutOfCollision(this, cell);
        }

        protected bool MoveOutOfCollision(FieldCell a, FieldCell b)
        {
            bool hadCollisiton = false;

            float overlapX = Constant.CELL_WIDTH - MathHelp.Abs(a.px - b.px);
            if (overlapX > 0 && MathHelp.Abs(a.oldPx - b.oldPx) >= Constant.CELL_WIDTH)
            {
                MovableCell ma = a.AsMovable();
                MovableCell mb = b.AsMovable();

                float moveA = Math.Abs(a.moveDx);
                float moveB = Math.Abs(b.moveDx);
                float kA = moveA / (moveA + moveB);
                float kB = 1 - kA;

                if (ma != null) ma.MoveBackX(kA * overlapX);
                if (mb != null) mb.MoveBackX(kB * overlapX);

                hadCollisiton = true;
            }

            float overlapY = Constant.CELL_HEIGHT - MathHelp.Abs(a.py - b.py);
            if (overlapY > 0 && MathHelp.Abs(a.oldPy - b.oldPy) >= Constant.CELL_HEIGHT)
            {
                MovableCell ma = a.AsMovable();
                MovableCell mb = b.AsMovable();

                float moveA = Math.Abs(a.moveDy);
                float moveB = Math.Abs(b.moveDy);
                float kA = moveA / (moveA + moveB);
                float kB = 1 - kA;

                if (ma != null) ma.MoveBackY(kA * overlapY);
                if (mb != null) mb.MoveBackY(kB * overlapY);

                hadCollisiton = true;
            }

            return hadCollisiton;
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
            m_moveKx = 0;
            m_moveKy = 0;
        }

        public void SetDirection(Direction newDirection)
        {   
            m_oldDirection = m_direction;
            m_direction = newDirection;
        }

        public void MoveBackX(float distance)
        {
            SetPosX(px - MathHelp.Sign(moveDx) * distance);
        }

        public void MoveBackY(float distance)
        {
            SetPosY(py - MathHelp.Sign(moveDy) * distance);
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

                moveDx = dx;
                moveDy = dy;

                // check collisions and all the stuff
                OnPositionChanged();

                moveDx = moveDy = 0.0f;

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

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

        public bool addedToList;

        public CellContactList contactList;

        public MovableCell(FieldCellType type, int cx, int cy)
            : base(type, cx, cy)
        {
            m_direction = Direction.DOWN;
            m_oldDirection = Direction.DOWN;

            contactList = new CellContactList(this);
        }

        public override void Update(float delta)
        {
            oldPx = px;
            oldPy = py;
        }

        public override void Reset()
        {
            base.Reset();

            StopMoving();

            m_direction = Direction.DOWN;
            m_oldDirection = Direction.DOWN;
            m_speed = 0.0f;
            addedToList = false;

            contactList.Clear();
        }

        public virtual void UpdateMoving(float delta)
        {   
            float offset = m_speed * delta;

            float dx = 0;
            float dy = 0;

            switch (direction)
            {
                case Direction.LEFT:
                case Direction.RIGHT:
                    {   
                        dx = m_moveKx * offset;
                        dy = GetTargetDy(delta);
                        break;
                    }

                case Direction.UP:
                case Direction.DOWN:
                    {
                        dx = GetTargetDx(delta);
                        dy = m_moveKy * offset;
                        break;
                    }
            }

            Move(dx, dy);
        }

        //private bool TryComeRoundObstacleX(float delta, int step)
        //{
        //    FieldCell blockingCell = NearCell(step, 0);
        //    if (blockingCell != null)
        //    {
        //        if (blockingCell.IsObstacle())
        //        {
        //            float blockingPy = blockingCell.GetPy();
        //            if (py < blockingPy && !GetField().IsObstacleCell(cx + step, cy - 1))
        //            {
        //                MoveY(Math.Max(Util.Cy2Py(cy - 1) - py, -m_speed * delta));
        //                return true;
        //            }
        //            if (py > blockingPy && !GetField().IsObstacleCell(cx + step, cy + 1))
        //            {
        //                MoveY(Math.Min(Util.Cy2Py(cy + 1) - py, m_speed * delta));
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        //private bool TryComeRoundObstacleY(float delta, int step)
        //{
        //    FieldCell blockingCell = NearCell(0, step);
        //    if (blockingCell != null)
        //    {
        //        if (blockingCell.IsObstacle())
        //        {
        //            float blockingPx = blockingCell.GetPx();
        //            if (px < blockingPx && !GetField().IsObstacleCell(cx - 1, cy + step))
        //            {
        //                MoveX(Math.Max(Util.Cx2Px(cx - 1) - px, -m_speed * delta));
        //                return true;
        //            }
        //            if (px > blockingPx && !GetField().IsObstacleCell(cx + 1, cy + step))
        //            {
        //                MoveX(Math.Min(Util.Cx2Px(cx + 1) - px, m_speed * delta));
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        private float GetTargetDx(float delta)
        {
            float xOffset = Util.TargetPxOffset(px, m_oldDirection);
            return xOffset < 0 ? Math.Max(xOffset, -delta * m_speed) : Math.Min(xOffset, delta * m_speed);
        }

        private float GetTargetDy(float delta)
        {
            float yOffset = Util.TargetPyOffset(py, m_oldDirection);
            return yOffset < 0 ? Math.Max(yOffset, -delta * m_speed) : Math.Min(yOffset, delta * m_speed);
        }

        public virtual bool HandleCollision(FieldCell other)
        {
            return false;
        }

        public virtual bool HandleWallCollision()
        {
            StopMoving();
            return true;
        }

        public virtual bool HandleObstacleCollision(FieldCell cell)
        {
            return MoveOutOfCollision(this, cell);
        }

        /* Move both cells out of collision */
        public static bool MoveOutOfCollision(FieldCell a, FieldCell b)
        {
            bool hadCollisiton = false;

            float overlapX = OverlapX(a, b);
            if (overlapX > 0 && Math.Abs(a.oldPx - b.oldPx) >= Constant.CELL_WIDTH)
            {
                MovableCell ma = a.AsMovable();
                MovableCell mb = b.AsMovable();

                if (ma == null || !ma.IsMoving())
                {
                    mb.MoveBackX(overlapX);
                }
                else if (mb == null || !mb.IsMoving())
                {
                    ma.MoveBackX(overlapX);
                }
                else
                {
                    float speedA = ma.GetSpeedX();
                    float speedB = mb.GetSpeedX();
                    float speedDiff = Math.Abs(speedA - speedB);
                    float time = overlapX / speedDiff;
                    float overlapA = time * ma.GetSpeed();
                    float overlapB = time * mb.GetSpeed();
                    if (overlapA > Math.Abs(ma.moveDx))
                    {
                        overlapA = Math.Abs(ma.moveDx);
                        overlapB = overlapX - overlapA;
                    }
                    else if (overlapB > Math.Abs(mb.moveDx))
                    {
                        overlapB = Math.Abs(mb.moveDx);
                        overlapA = overlapX - overlapB;
                    }

                    ma.MoveBackX(overlapA);
                    mb.MoveBackX(overlapB);
                }

                hadCollisiton = true;
            }

            float overlapY = OverlapY(a, b);
            if (overlapY > 0 && Math.Abs(a.oldPy - b.oldPy) >= Constant.CELL_HEIGHT)
            {
                MovableCell ma = a.AsMovable();
                MovableCell mb = b.AsMovable();

                if (ma == null || !ma.IsMoving())
                {
                    mb.MoveBackY(overlapY);
                }
                else if (mb == null || !mb.IsMoving())
                {
                    ma.MoveBackY(overlapY);
                }
                else
                {
                    float speedA = ma.GetSpeedY();
                    float speedB = mb.GetSpeedY();
                    float speedDiff = Math.Abs(speedA - speedB);
                    float time = overlapY / speedDiff;
                    float overlapA = time * ma.GetSpeed();
                    float overlapB = time * mb.GetSpeed();
                    if (overlapA > Math.Abs(ma.moveDy))
                    {
                        overlapA = Math.Abs(ma.moveDy);
                        overlapB = overlapY - overlapA;
                    }
                    else if (overlapB > Math.Abs(mb.moveDy))
                    {
                        overlapB = Math.Abs(mb.moveDy);
                        overlapA = overlapY - overlapB;
                    }
                    ma.MoveBackY(overlapA);
                    mb.MoveBackY(overlapB);
                }

                hadCollisiton = true;
            }

            return hadCollisiton;
        }

        public bool MoveOutOfCollision(FieldCell cell)
        {
            bool hadCollisiton = false;

            float overlapX = OverlapX(this, cell);
            if (overlapX > 0 && Math.Abs(oldPx - cell.oldPx) >= Constant.CELL_WIDTH)
            {
                MoveBackX(overlapX);
                hadCollisiton = true;
            }

            float overlapY = OverlapY(this, cell);
            if (overlapY > 0 && Math.Abs(oldPy - cell.oldPy) >= Constant.CELL_HEIGHT)
            {
                MoveBackY(overlapY);
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

        public bool TryStopKicked()
        {
            if (m_moving)
            {
                SetCell();
                StopMoving();

                return true;
            }

            return false;
        }

        public void SetDirection(Direction newDirection)
        {   
            m_oldDirection = m_direction;
            m_direction = newDirection;
        }

        public void MoveBackX(float distance)
        {
            MoveX(-m_moveKx * distance);
        }

        public void MoveBackY(float distance)
        {
            MoveY(-m_moveKy * distance);
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
                SetPos(px + dx, py + dy);

                // check collisions and all the stuff
                OnCellMoved();
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

        public override void SetPos(float px, float py)
        {
            int oldCx = cx;
            int oldCy = cy;

            base.SetPos(px, py);

            if (oldCx != cx || oldCy != cy)
            {
                OnCellChanged(oldCx, oldCy);
            }
        }

        public void SetRelativeTo(FieldCell cell, int stepX, int stepY)
        {
            float posX = stepX != 0 ? (cell.px + stepX * Constant.CELL_WIDTH) : px;
            float posY = stepY != 0 ? (cell.py + stepY * Constant.CELL_HEIGHT) : py;

            SetPos(posX, posY);
        }

        protected virtual void OnCellMoved()
        {   
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

        public float GetSpeedX()
        {
            return m_speed * m_moveKx;
        }

        public float GetSpeedY()
        {
            return m_speed * m_moveKy;
        }

        public override bool IsMovable()
        {
            return true;
        }

        public override MovableCell AsMovable()
        {
            return this;
        }

        public virtual bool IsMoving()
        {
            return m_moving;
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

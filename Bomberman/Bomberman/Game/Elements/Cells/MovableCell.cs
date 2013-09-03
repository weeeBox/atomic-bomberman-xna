using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;
using BomberEngine.Util;
using BomberEngine.Debugging;

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

        public MovableCell(FieldCellType type, int cx, int cy)
            : base(type, cx, cy)
        {
            m_direction = Direction.DOWN;
            m_oldDirection = Direction.DOWN;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region IResettable

        public override void Reset()
        {
            base.Reset();

            StopMoving();

            m_direction = Direction.DOWN;
            m_oldDirection = Direction.DOWN;
            m_speed = 0.0f;
            addedToList = false;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IUpdatable

        public override void Update(float delta)
        {
            oldPx = px;
            oldPy = py;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Cell

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

        public float CenterOffX
        {
            get { return Util.CellCenterOffX(px); }
        }

        public float CenterOffY
        {
            get { return Util.CellCenterOffY(py); }
        }

        #endregion
        
        //////////////////////////////////////////////////////////////////////////////

        #region Movable

        public virtual void UpdateMoving(float delta)
        {   
            float offset = m_speed * delta;

            float dx = 0;
            float dy = 0;

            float oldPx = px;
            float oldPy = py;

            switch (direction)
            {
                case Direction.LEFT:
                case Direction.RIGHT:
                {   
                    dx = m_moveKx * offset;
                    break;
                }

                case Direction.UP:
                case Direction.DOWN:
                {   
                    dy = m_moveKy * offset;
                    break;
                }
            }

            Move(dx, dy);

            // we need to calculate static collisions right away to check if player's
            // movement is blocked by a static or a still object
            GetField().CheckStaticCollisions(this);
            
            switch (direction)
            {
                case Direction.LEFT:
                case Direction.RIGHT:
                {
                    bool blocked = Math.Abs(px - oldPx) < 0.01f;
                    dx = 0.0f;
                    dy = GetMoveTargetDy(delta, blocked);
                    break;
                }

                case Direction.UP:
                case Direction.DOWN:
                {
                    bool blocked = Math.Abs(px - oldPx) < 0.01f;
                    dx = GetMoveTargetDx(delta, blocked);
                    dy = 0.0f;
                    break;
                }
            }

            Move(dx, dy);
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

        public virtual void StopMoving()
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

        protected virtual float GetMoveTargetDx(float delta, bool blocked)
        {   
            float xOffset = Util.TargetPxOffset(px);
            return xOffset < 0 ? Math.Max(xOffset, -delta * m_speed) : Math.Min(xOffset, delta * m_speed);
        }

        protected virtual float GetMoveTargetDy(float delta, bool blocked)
        {
            float yOffset = Util.TargetPyOffset(py);
            return yOffset < 0 ? Math.Max(yOffset, -delta * m_speed) : Math.Min(yOffset, delta * m_speed);
        }

        public void SetSpeed(float speed)
        {
            m_speed = speed;
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

        public bool IsMoving()
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

        protected float moveKx
        {
            get { return m_moveKx; }
        }

        protected float moveKy
        {
            get { return m_moveKy; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////
        
        #region Virtual Type

        public override bool IsMovable()
        {
            return true;
        }

        public override MovableCell AsMovable()
        {
            return this;
        }

        #endregion
        
        //////////////////////////////////////////////////////////////////////////////

        #region Collider

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

        #endregion
    }
}

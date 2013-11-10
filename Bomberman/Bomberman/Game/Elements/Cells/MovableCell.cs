using System;
using BomberEngine;
using Bomberman.Gameplay.Elements.Fields;

namespace Bomberman.Gameplay.Elements.Cells
{
    public class MovableCell : FieldCell
    {
        private Direction m_direction;

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
        }

        //////////////////////////////////////////////////////////////////////////////

        #region IResettable

        public override void Reset()
        {
            base.Reset();

            StopMoving();

            m_direction = Direction.DOWN;
            m_speed = 0.0f;
            addedToList = false;
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
            float posX = stepX != 0 ? (cell.CellCenterPx() + stepX * Constant.CELL_WIDTH) : px;
            float posY = stepY != 0 ? (cell.CellCenterPy() + stepY * Constant.CELL_HEIGHT) : py;

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
            Assert.IsTrue(IsMoving());

            float offset = GetSpeed() * delta;

            float dx = 0;
            float dy = 0;

            switch (direction)
            {
                case Direction.LEFT:
                case Direction.RIGHT:
                {   
                    dx = moveKx * offset;
                    dy = GetMoveTargetDy(delta);
                    break;
                }

                case Direction.UP:
                case Direction.DOWN:
                {
                    dx = GetMoveTargetDx(delta);
                    dy = moveKy * offset;
                    break;
                }
            }

            Move(dx, dy);
        }

        protected void SetMoveDirection(Direction direction)
        {
            SetDirection(direction);
            m_moving = true;

            m_moveKx = 0;
            m_moveKy = 0;

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
            m_direction = newDirection;
        }

        public void MoveFromOverlap(FieldCell other)
        {
            Assert.IsTrue(IsMoving());

            float dx = OverlapX(other);
            float dy = OverlapY(other);

            MoveBackX(dx);
            MoveBackY(dy);
        }

        public void MoveOutOfCell(FieldCell c)
        {
            Assert.IsTrue(IsMoving());

            switch (direction)
            {
                case Direction.LEFT:
                    Assert.IsTrue(c.px < px);
                    if (px - c.CellCenterPx() < Constant.CELL_WIDTH)
                    {
                        SetRelativeTo(c, 1, 0);
                    }
                    break;

                case Direction.RIGHT:
                    Assert.IsTrue(c.px > px);
                    if (c.CellCenterPx() - px < Constant.CELL_WIDTH)
                    {
                        SetRelativeTo(c, -1, 0);
                    }
                    break;

                case Direction.UP:
                    Assert.IsTrue(c.py < py);
                    if (py - c.CellCenterPy() < Constant.CELL_HEIGHT)
                    {
                        SetRelativeTo(c, 0, 1);
                    }
                    break;

                case Direction.DOWN:
                    Assert.IsTrue(c.py > py);
                    if (c.CellCenterPy() - py < Constant.CELL_HEIGHT)
                    {
                        SetRelativeTo(c, 0, -1);
                    }
                    break;
            }
        }

        public bool IsMovingTowards(FieldCell other)
        {
            Assert.IsTrue(IsMoving());
            
            switch (direction)
            {
                case Direction.LEFT:
                    return px > other.px;

                case Direction.RIGHT:
                    return px < other.px;

                case Direction.UP:
                    return py > other.py;

                case Direction.DOWN:
                    return py < other.py;
            }

            return false;
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

        protected float GetMoveTargetDx(float delta)
        {   
            float xOffset = Util.TargetPxOffset(px);
            return xOffset < 0 ? Math.Max(xOffset, -delta * m_speed) : Math.Min(xOffset, delta * m_speed);
        }

        protected float GetMoveTargetDy(float delta)
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

        public float moveKx
        {
            get { return m_moveKx; }
        }

        public float moveKy
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

        public virtual bool HandleCollision(FieldCell cell)
        {
            if (cell.IsMovable())
            {
                return HandleCollision(cell.AsMovable());
            }
            
            return HandleStaticCollision(cell);
        }

        /* Movable cell */
        protected virtual bool HandleCollision(MovableCell other)
        {
            return false;
        }

        /* Not movable cell */
        protected virtual bool HandleStaticCollision(FieldCell other)
        {
            if (other.IsObstacle())
            {
                MoveFromOverlap(other);
                return true;
            }

            return false;
        }

        public virtual bool HandleWallCollision()
        {
            return false;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Equality

        public override bool EqualsTo(FieldCell other)
        {
            MovableCell movable = other as MovableCell;
            return base.EqualsTo(movable) &&
                movable.m_direction == m_direction &&
                movable.m_speed == m_speed &&
                movable.m_moveKx == m_moveKx &&
                movable.m_moveKy == m_moveKy &&
                movable.m_moving == m_moving;
        }

        #endregion
    }
}

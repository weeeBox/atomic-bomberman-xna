using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Cells
{
    public class MovableCell : FieldCell
    {
        private Direction direction;
        private Direction oldDirection;

        /* Points per second */
        protected float speed;

        public MovableCell(int cx, int cy)
            : base(cx, cy)
        {
            direction = Direction.DOWN;
            oldDirection = Direction.DOWN;
        }

        public Direction GetDirection()
        {
            return direction;
        }

        public Direction GetOldDirection()
        {
            return oldDirection;
        }

        public void SetDirection(Direction newDirection)
        {   
            oldDirection = direction;
            direction = newDirection;
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
                float oldPx = px;
                float oldPy = py;

                px += dx;
                py += dy;

                OnPositionChanged(oldPx, oldPy);
                UpdateCellPos(px, py);
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
            this.px = px;
            this.py = py;

            UpdateCellPos(px, py);
        }

        private void UpdateCellPos(float px, float py)
        {
            cx = Util.Px2Cx(px);
            cy = Util.Py2Cy(py);
        }

        protected void OnPositionChanged(float oldPx, float oldPy)
        {
            Field.Current().CellPosChanged(this, oldPx, oldPy);
        }

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        public void IncSpeed(float amount)
        {
            SetSpeed(speed + amount);
        }

        public float GetSpeed()
        {
            return speed;
        }

        public virtual bool IsBomb()
        {
            return false;
        }

        public virtual bool IsPlayer()
        {
            return false;
        }
    }
}

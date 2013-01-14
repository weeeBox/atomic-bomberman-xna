using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Cells
{
    public class MovableCell : FieldCell
    {
        protected Direction direction;

        /* Points coordinates */
        protected float px;
        protected float py;

        /* Points per second */
        protected float speed;

        public MovableCell(int x, int y)
            : base(x, y)
        {
            direction = Direction.DOWN;
        }

        public Direction GetDirection()
        {
            return direction;
        }

        public void SetDirection(Direction direction)
        {
            this.direction = direction;
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
            SetPx(px + dx, py + dy);
        }

        public void SetPx(float px, float py)
        {
            float oldPx = px;
            float oldPy = py;

            this.px = px;
            this.py = py;

            if (oldPx != px || oldPy != py)
            {
                OnPositionChanged(oldPx, oldPy);
            }

            int oldX = x;
            int oldY = y;

            x = (int)(px / Constant.CELL_WIDTH + 0.5f);
            y = (int)(py / Constant.CELL_HEIGHT + 0.5f);

            if (oldX != x || oldY != y)
            {
                OnCellChanged(oldX, oldY);
            }
        }

        protected void OnPositionChanged(float oldPx, float oldPy)
        {
            Field.Current().CellPointsPosChanged(this, oldPx, oldPy);
        }

        protected void OnCellChanged(int oldX, int oldY)
        {
            Field.Current().CellCoordChanged(this, oldX, oldY);
        }
        
        public float GetPx()
        {
            return px;
        }

        public float GetPy()
        {
            return py;
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

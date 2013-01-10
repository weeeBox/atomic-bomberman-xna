using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Game.Elements.Cells
{
    public class MovingCell : FieldCell
    {
        private Direction direction;

        /* Points coordinates */
        private float px;
        private float py;

        /* Points per second */
        private float speed;

        public MovingCell(int x, int y)
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

        public void SetPx(float x, float y)
        {
            px = x;
            py = y;
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
    }
}

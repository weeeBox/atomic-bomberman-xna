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
        public float speed;

        public bool moving;

        /* The last position BEFORE MOVE operation */
        public float oldPx;
        public float oldPy;

        public MovableCell(int cx, int cy)
            : base(cx, cy)
        {
            direction = Direction.DOWN;
            oldDirection = Direction.DOWN;
        }

        public override void Update(float delta)
        {
            if (moving)
            {
                UpdateMoving(delta);
            }
        }

        private void UpdateMoving(float delta)
        {
            float oldPx = px;
            float oldPy = py;

            Direction direction = GetDirection();
            switch (direction)
            {
                case Direction.UP:
                    {
                        MoveY(-delta * speed);

                        // TODO: разобрать это пиздец
                        FieldCell blockingCell = null;
                        if (oldPy == py) // movement forward blocked?
                        {
                            blockingCell = GetField().GetCell(cx, cy - 1);
                            if (blockingCell != null && blockingCell.IsObstacle())
                            {
                                float blockingPx = blockingCell.GetPx();
                                if (px < blockingPx && !GetField().IsObstacleCell(cx - 1, cy - 1))
                                {
                                    MoveX(Math.Max(Util.Cx2Px(cx - 1) - px, -speed * delta));
                                }
                                else if (px > blockingPx && !GetField().IsObstacleCell(cx + 1, cy - 1))
                                {
                                    MoveX(Math.Min(Util.Cx2Px(cx + 1) - px, speed * delta));
                                }
                            }
                            else
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

                case Direction.DOWN:
                    {
                        MoveY(delta * speed);

                        // TODO: разобрать это пиздец
                        FieldCell blockingCell = null;
                        if (oldPy == py) // movement forward blocked?
                        {
                            blockingCell = GetField().GetCell(cx, cy + 1);
                            if (blockingCell != null && blockingCell.IsObstacle())
                            {
                                float blockingPx = blockingCell.GetPx();
                                if (px < blockingPx && !GetField().IsObstacleCell(cx - 1, cy + 1))
                                {
                                    MoveX(Math.Max(Util.Cx2Px(cx - 1) - px, -speed * delta));
                                }
                                else if (px > blockingPx && !GetField().IsObstacleCell(cx + 1, cy + 1))
                                {
                                    MoveX(Math.Min(Util.Cx2Px(cx + 1) - px, speed * delta));
                                }
                            }
                            else
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

                case Direction.LEFT:
                    {
                        MoveX(-delta * speed);

                        // TODO: разобрать это пиздец
                        FieldCell blockingCell = null;
                        if (oldPx == px) // movement forward blocked?
                        {
                            blockingCell = GetField().GetCell(cx - 1, cy);
                            if (blockingCell != null && blockingCell.IsObstacle())
                            {
                                float blockingPy = blockingCell.GetPy();
                                if (py < blockingPy && !GetField().IsObstacleCell(cx - 1, cy - 1))
                                {
                                    MoveY(Math.Max(Util.Cy2Py(cy - 1) - py, -speed * delta));
                                }
                                else if (py > blockingPy && !GetField().IsObstacleCell(cx - 1, cy + 1))
                                {
                                    MoveY(Math.Min(Util.Cy2Py(cy + 1) - py, speed * delta));
                                }
                            }
                            else
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

                case Direction.RIGHT:
                    {
                        MoveX(delta * speed);

                        // TODO: разобрать это пиздец
                        FieldCell blockingCell = null;
                        if (oldPx == px) // movement forward blocked?
                        {
                            blockingCell = GetField().GetCell(cx + 1, cy);
                            if (blockingCell != null && blockingCell.IsObstacle())
                            {
                                float blockingPy = blockingCell.GetPy();
                                if (py < blockingPy && !GetField().IsObstacleCell(cx + 1, cy - 1))
                                {
                                    MoveY(Math.Max(Util.Cy2Py(cy - 1) - py, -speed * delta));
                                }
                                else if (py > blockingPy && !GetField().IsObstacleCell(cx + 1, cy + 1))
                                {
                                    MoveY(Math.Min(Util.Cy2Py(cy + 1) - py, speed * delta));
                                }
                            }
                            else
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
            }
        }

        private void MoveToTargetPx(float delta)
        {
            float xOffset = Util.TargetPxOffset(px);
            MoveX(xOffset < 0 ? Math.Max(xOffset, -delta * speed) : Math.Min(xOffset, delta * speed));
        }

        private void MoveToTargetPy(float delta)
        {
            float yOffset = Util.TargetPyOffset(py);
            MoveY(yOffset < 0 ? Math.Max(yOffset, -delta * speed) : Math.Min(yOffset, delta * speed));
        }

        public virtual void HitWall()
        {
            StopMoving();
        }

        public virtual void HitObstacle(FieldCell obstacle)
        {
            StopMoving();
        }

        protected void SetMoveDirection(Direction direction)
        {
            SetDirection(direction);
            moving = true;
        }

        public void StopMoving()
        {
            moving = false;
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
                oldPx = px;
                oldPy = py;

                px += dx;
                py += dy;

                OnPositionChanged();
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

            oldPx = px;
            oldPy = py;

            UpdateCellPos(px, py);
        }

        private void UpdateCellPos(float px, float py)
        {
            cx = Util.Px2Cx(px);
            cy = Util.Py2Cy(py);
        }

        protected void OnPositionChanged()
        {
            GetField().CellPosChanged(this);
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

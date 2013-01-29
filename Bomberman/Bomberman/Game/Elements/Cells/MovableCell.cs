using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Cells
{
    public class MovableCell : FieldCell
    {
        private Direction m_direction;
        private Direction m_oldDirection;

        /* Points per second */
        private float m_speed;

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

        private void UpdateMoving(float delta)
        {
            float oldPx = px;
            float oldPy = py;
            
            switch (direction)
            {
                case Direction.UP:
                    {
                        MoveY(-delta * m_speed);

                        // TODO: разобрать это пиздец
                        FieldCell blockingCell = null;
                        if (oldPy == py) // movement forward blocked?
                        {
                            blockingCell = NearCell(0, -1);
                            if (blockingCell == null)
                            {
                                break; // hit the wall
                            }

                            if (blockingCell.IsObstacle())
                            {
                                float blockingPx = blockingCell.GetPx();
                                if (px < blockingPx && !GetField().IsObstacleCell(cx - 1, cy - 1))
                                {
                                    MoveX(Math.Max(Util.Cx2Px(cx - 1) - px, -m_speed * delta));
                                }
                                else if (px > blockingPx && !GetField().IsObstacleCell(cx + 1, cy - 1))
                                {
                                    MoveX(Math.Min(Util.Cx2Px(cx + 1) - px, m_speed * delta));
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
                        MoveY(delta * m_speed);

                        // TODO: разобрать это пиздец
                        FieldCell blockingCell = null;
                        if (oldPy == py) // movement forward blocked?
                        {
                            blockingCell = NearCell(0, 1);
                            if (blockingCell == null)
                            {
                                break; // hit the wall
                            }

                            if (blockingCell.IsObstacle())
                            {
                                float blockingPx = blockingCell.GetPx();
                                if (px < blockingPx && !GetField().IsObstacleCell(cx - 1, cy + 1))
                                {
                                    MoveX(Math.Max(Util.Cx2Px(cx - 1) - px, -m_speed * delta));
                                }
                                else if (px > blockingPx && !GetField().IsObstacleCell(cx + 1, cy + 1))
                                {
                                    MoveX(Math.Min(Util.Cx2Px(cx + 1) - px, m_speed * delta));
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
                        MoveX(-delta * m_speed);

                        // TODO: разобрать это пиздец
                        FieldCell blockingCell = null;
                        if (oldPx == px) // movement forward blocked?
                        {
                            blockingCell = NearCell(-1, 0);
                            if (blockingCell == null)
                            {
                                break; // hit the wall
                            }

                            if (blockingCell.IsObstacle())
                            {
                                float blockingPy = blockingCell.GetPy();
                                if (py < blockingPy && !GetField().IsObstacleCell(cx - 1, cy - 1))
                                {
                                    MoveY(Math.Max(Util.Cy2Py(cy - 1) - py, -m_speed * delta));
                                }
                                else if (py > blockingPy && !GetField().IsObstacleCell(cx - 1, cy + 1))
                                {
                                    MoveY(Math.Min(Util.Cy2Py(cy + 1) - py, m_speed * delta));
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
                        MoveX(delta * m_speed);

                        // TODO: разобрать это пиздец
                        FieldCell blockingCell = null;
                        if (oldPx == px) // movement forward blocked?
                        {
                            blockingCell = NearCell(1, 0);
                            if (blockingCell == null)
                            {
                                break; // hit the wall
                            }

                            if (blockingCell.IsObstacle())
                            {
                                float blockingPy = blockingCell.GetPy();
                                if (py < blockingPy && !GetField().IsObstacleCell(cx + 1, cy - 1))
                                {
                                    MoveY(Math.Max(Util.Cy2Py(cy - 1) - py, -m_speed * delta));
                                }
                                else if (py > blockingPy && !GetField().IsObstacleCell(cx + 1, cy + 1))
                                {
                                    MoveY(Math.Min(Util.Cy2Py(cy + 1) - py, m_speed * delta));
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
            MoveX(xOffset < 0 ? Math.Max(xOffset, -delta * m_speed) : Math.Min(xOffset, delta * m_speed));
        }

        private void MoveToTargetPy(float delta)
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
        }

        public void StopMoving()
        {
            m_moving = false;
        }

        public void SetDirection(Direction newDirection)
        {   
            m_oldDirection = m_direction;
            m_direction = newDirection;
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
                
                // update pixel cords without updating cell cords
                SetPixelCords(px + dx, py + dy);

                // check collisions and all the stuff
                OnPositionChanged(oldPx, oldPy);

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

        protected virtual void OnPositionChanged(float oldPx, float oldPy)
        {
            GetField().MoveablePosChanged(this, oldPx, oldPy);
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

        public float speed
        {
            get { return m_speed; }
        }
    }
}

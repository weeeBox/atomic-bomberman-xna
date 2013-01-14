using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Items;
using Bomberman.Game.Elements.Players.Input;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Players
{
    public class Player : MovableCell, PlayerInputListener
    {
        private static readonly int SPEED_START = 145;
        private static readonly int SPEED_INCREASE = 25;

        private bool alive;
        private bool moving;

        private int bombRadius;
        private float bombTimeout;
        private bool bombBouncing;
        private bool bombDetonated;

        private PlayerInput input;

        public Player(PlayerInput input, int x, int y)
            : base(x, y)
        {
            this.input = input;
            input.SetListener(this);

            alive = true;
            speed = SPEED_START;
        }

        public override void Update(float delta)
        {
            if (moving)
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
                                float xOffset = Util.TargetPxOffset(px);
                                MoveX(xOffset < 0 ? Math.Max(xOffset, -delta * speed) : Math.Min(xOffset, delta * speed));
                            }
                        }
                        else                        
                        {
                            float xOffset = Util.TargetPxOffset(px);
                            MoveX(xOffset < 0 ? Math.Max(xOffset, -delta * speed) : Math.Min(xOffset, delta * speed));
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
                                float xOffset = Util.TargetPxOffset(px);
                                MoveX(xOffset < 0 ? Math.Max(xOffset, -delta * speed) : Math.Min(xOffset, delta * speed));
                            }
                        }
                        else
                        {
                            float xOffset = Util.TargetPxOffset(px);
                            MoveX(xOffset < 0 ? Math.Max(xOffset, -delta * speed) : Math.Min(xOffset, delta * speed));
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
                                float yOffset = Util.TargetPyOffset(py);
                                MoveY(yOffset < 0 ? Math.Max(yOffset, -delta * speed) : Math.Min(yOffset, delta * speed));
                            }
                        }
                        else
                        {
                            float yOffset = Util.TargetPyOffset(py);
                            MoveY(yOffset < 0 ? Math.Max(yOffset, -delta * speed) : Math.Min(yOffset, delta * speed));
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
                                float yOffset = Util.TargetPyOffset(py);
                                MoveY(yOffset < 0 ? Math.Max(yOffset, -delta * speed) : Math.Min(yOffset, delta * speed));
                            }
                        }
                        else
                        {
                            float yOffset = Util.TargetPyOffset(py);
                            MoveY(yOffset < 0 ? Math.Max(yOffset, -delta * speed) : Math.Min(yOffset, delta * speed));
                        }
                        break;
                    }
                }
            }
        }

        public void OnActionPressed(PlayerInput playerInput, PlayerAction action)
        {
            switch (action)
            {
                case PlayerAction.Up:
                {
                    SetMoveDirection(Direction.UP);
                    break;
                }

                case PlayerAction.Down:
                {
                    SetMoveDirection(Direction.DOWN);
                    break;
                }

                case PlayerAction.Left:
                {
                    SetMoveDirection(Direction.LEFT);
                    break;
                }

                case PlayerAction.Right:
                {
                    SetMoveDirection(Direction.RIGHT);
                    break;
                }

                case PlayerAction.Bomb:
                {
                    SetBomb();
                    break;
                }
            }
        }

        public void OnActionReleased(PlayerInput playerInput, PlayerAction action)
        {
            moving = false;

            if (playerInput.GetPressedActionCount() > 0)
            {
                if (playerInput.IsActionPressed(PlayerAction.Up))
                {
                    SetMoveDirection(Direction.UP);
                }
                else if (playerInput.IsActionPressed(PlayerAction.Down))
                {
                    SetMoveDirection(Direction.DOWN);
                }
                else if (playerInput.IsActionPressed(PlayerAction.Left))
                {
                    SetMoveDirection(Direction.LEFT);
                }
                else if (playerInput.IsActionPressed(PlayerAction.Right))
                {
                    SetMoveDirection(Direction.RIGHT);
                }
            }
        }

        public void OnActonsReleased(PlayerInput playerInput)
        {
            moving = false;
        }

        private void SetMoveDirection(Direction direction)
        {
            SetDirection(direction);
            moving = true;
        }

        public override bool IsPlayer()
        {
            return true;
        }

        public bool IsAlive()
        {
            return alive;
        }

        public Bomb SetBomb()
        {
            return new Bomb(this, false); // TODO: calculate dud
        }

        public float GetBombTimeout()
        {
            return bombTimeout;
        }

        public int GetBombRadius()
        {
            return bombRadius;
        }

        public bool IsBombBouncing()
        {
            return bombBouncing;
        }

        public bool IsBobmDetonated()
        {
            return bombDetonated;
        }
    }
}

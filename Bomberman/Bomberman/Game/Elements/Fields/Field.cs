using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Visual;
using Assets;
using BomberEngine.Game;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Cells;
using BomberEngine.Debugging;

namespace Bomberman.Game.Elements.Fields
{
    public class Field : Updatable
    {
        private FieldCellArray cells;

        private PlayerArray players;

        private static Field currentField;

        public Field(int width, int height)
        {
            currentField = this;

            cells = new FieldCellArray(width, height);
            players = new PlayerArray();
        }

        public void Update(float delta)
        {
            players.Update(delta);
        }

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public PlayerArray GetPlayers()
        {
            return players;
        }

        public FieldCell GetCell(int cx, int cy)
        {
            return cells.Get(cx, cy);
        }

        public bool IsObstacleCell(int cx, int cy)
        {
            FieldCell cell = GetCell(cx, cy);
            return cell == null || cell.IsObstacle();
        }

        public FieldCellArray GetCells()
        {
            return cells;
        }

        public int GetWidth()
        {
            return cells.GetWidth();
        }

        public int GetHeight()
        {
            return cells.GetHeight();
        }

        public void CellPosChanged(MovableCell cell, float oldPx, float oldPy)
        {
            ProcessFieldBounds(cell);
            ProcessCollisions(cell, oldPx, oldPy);
        }

        private static void ProcessFieldBounds(MovableCell cell)
        {
            Direction direction = cell.GetDirection();
            switch (direction)
            {
                case Direction.LEFT:
                {
                    float minX = 0.5f * Constant.CELL_WIDTH;
                    if (cell.GetPx() < minX)
                    {
                        cell.SetPosX(minX);
                    }
                    break;
                }

                case Direction.RIGHT:
                {
                    float maxX = Constant.FIELD_WIDTH - 0.5f * Constant.CELL_WIDTH;
                    if (cell.GetPx() > maxX)
                    {
                        cell.SetPosX(maxX);
                    }
                    break;
                }

                case Direction.UP:
                {
                    float minY = 0.5f * Constant.CELL_HEIGHT;
                    if (cell.GetPy() < minY)
                    {
                        cell.SetPosY(minY);
                    }
                    break;
                }

                case Direction.DOWN:
                {
                    float maxY = Constant.FIELD_HEIGHT - 0.5f * Constant.CELL_HEIGHT;
                    if (cell.GetPy() > maxY)
                    {
                        cell.SetPosY(maxY);
                    }
                    break;
                }
            }
        }

        private void ProcessCollisions(MovableCell cell, float oldPx, float oldPy)
        {
            float px = cell.GetPx();
            float py = cell.GetPy();
            float dx = px - oldPx;
            float dy = py - oldPy;

            if (dx > 0)
            {
                int cx = Util.Px2Cx(px) + 1;
                int cy = Util.Py2Cy(py);

                ProcessCollision(cell, cx, cy - 1);
                ProcessCollision(cell, cx, cy);
                ProcessCollision(cell, cx, cy + 1);
            }
            else if (dx < 0)
            {
                int cx = Util.Px2Cx(px) - 1;
                int cy = Util.Py2Cy(py);

                ProcessCollision(cell, cx, cy - 1);
                ProcessCollision(cell, cx, cy);
                ProcessCollision(cell, cx, cy + 1);
            }

            if (dy > 0)
            {
                int cx = Util.Px2Cx(px);
                int cy = Util.Py2Cy(py) + 1;

                ProcessCollision(cell, cx - 1, cy);
                ProcessCollision(cell, cx, cy);
                ProcessCollision(cell, cx + 1, cy);
            }
            else if (dy < 0)
            {
                int cx = Util.Px2Cx(px);
                int cy = Util.Py2Cy(py) - 1;

                ProcessCollision(cell, cx - 1, cy);
                ProcessCollision(cell, cx, cy);
                ProcessCollision(cell, cx + 1, cy);
            }
        }

        private void ProcessCollision(MovableCell cell, int cx, int cy)
        {
            if (cx >= 0 && cx < GetWidth() && cy >=0 && cy < GetHeight())
            {
                FieldCell other = cells.Get(cx, cy);

                if (other.IsObstacle())
                {
                    if (Collides(cell, other))
                    {
                        switch (cell.GetDirection())
                        {
                            case Direction.UP:
                            {
                                cell.SetPosY(Util.Cy2Py(cy + 1));
                                break;
                            }
                            case Direction.DOWN:
                            {
                                cell.SetPosY(Util.Cy2Py(cy - 1));
                                break;
                            }
                            case Direction.LEFT:
                            {
                                cell.SetPosX(Util.Cx2Px(cx + 1));
                                break;
                            }
                            case Direction.RIGHT:
                            {
                                cell.SetPosX(Util.Cx2Px(cx - 1));
                                break;
                            }
                        }
                    }
                }
            }
        }

        private bool Collides(MovableCell a, FieldCell b)
        {
            float acx = a.GetPx();
            float acy = a.GetPy();
            float bcx = b.GetPx();
            float bcy = b.GetPy();

            return Math.Abs(acx - bcx) < Constant.CELL_WIDTH && Math.Abs(acy - bcy) < Constant.CELL_HEIGHT;
        }

        public static Field Current()
        {
            return currentField;
        }
    }
}

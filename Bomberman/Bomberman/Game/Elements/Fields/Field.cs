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
using Bomberman.Game.Elements.Items;

namespace Bomberman.Game.Elements.Fields
{
    public class Field : Updatable
    {
        private FieldCellArray cells;

        private PlayerArray players;

        private static Field currentField;

        private TimerManager timerManager;

        public Field(int width, int height)
        {
            currentField = this;

            timerManager = new TimerManager();

            cells = new FieldCellArray(width, height);
            players = new PlayerArray();
        }

        public void Update(float delta)
        {
            timerManager.Update(delta);

            UpdateCells(delta);

            players.Update(delta);
        }

        private void UpdateCells(float delta)
        {
            FieldCell[] cellsArray = cells.GetArray();
            for (int i = 0; i < cellsArray.Length; ++i)
            {
                cellsArray[i].Update(delta);
            }
        }

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public void SetBomb(Bomb bomb)
        {
            cells.Set(bomb.GetCx(), bomb.GetCy(), bomb);
        }

        public void BlowBomb(Bomb bomb)
        {
            int cx = bomb.GetCx();
            int cy = bomb.GetCy();

            SetExplosion(cx, cy);
            SpreadExplosion(bomb, cx - 1, cy);
            SpreadExplosion(bomb, cx, cy - 1);
            SpreadExplosion(bomb, cx + 1, cy);
            SpreadExplosion(bomb, cx, cy + 1);
        }

        private void SpreadExplosion(Bomb bomb, int startCx, int startCy)
        {
            int dcx = startCx - bomb.GetCx();
            int dcy = startCy - bomb.GetCy();

            int radius = bomb.GetRadius();
            int cx = startCx;
            int cy = startCy;

            for (int i = 0; i < radius; ++i)
            {
                bool spreaded = SetExplosion(cx, cy);
                if (!spreaded)
                {
                    break;
                }

                SetExplosion(cx, cy);

                cx += dcx;
                cy += dcy;
            }
        }

        private bool SetExplosion(int cx, int cy)
        {
            FieldCell cell = GetCell(cx, cy);
            if (cell == null)
            {
                return false; // bomb hits the wall
            }

            if (cell.IsBreakable())
            {
                SetCell(new EmptyCell(cx, cy));
                return false; // bomb destroyed a brick
            }

            if (cell.IsSolid())
            {
                return false; // bomb hits solid block
            }

            SetCell(new ExplosionCell(cx, cy));
            return true;
        }

        public PlayerArray GetPlayers()
        {
            return players;
        }

        public FieldCell GetCell(int cx, int cy)
        {
            return cells.Get(cx, cy);
        }

        public void SetCell(FieldCell cell)
        {
            cells.Set(cell.GetCx(), cell.GetCy(), cell);
        }

        public void ClearCell(int cx, int cy)
        {
            SetCell(new EmptyCell(cx, cy));
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

        private bool IsInsideField(int cx, int cy)
        {
            return cx >= 0 && cx < GetWidth() && cy >= 0 && cy < GetHeight();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region TimerManager

        public Timer ScheduleTimer(TimerCallback callback, float delay)
        {
            return ScheduleTimer(callback, delay, false);
        }

        public Timer ScheduleTimer(TimerCallback callback, float delay, bool repeated)
        {
            return timerManager.Schedule(callback, delay, repeated);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public static Field Current()
        {
            return currentField;
        }
    }
}

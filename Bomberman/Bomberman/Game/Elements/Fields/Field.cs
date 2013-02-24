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
using Bomberman.Content;
using BombermanCommon.Resources.Scheme;
using BomberEngine.Util;

namespace Bomberman.Game.Elements.Fields
{
    public class Field : Updatable
    {
        private static Field currentField;

        private static readonly int[] POWERUPS_COUNT =
        {
            Settings.VAL_PU_FIELD_BOMB,
            Settings.VAL_PU_FIELD_FLAME,
            Settings.VAL_PU_FIELD_DISEASE,
            Settings.VAL_PU_FIELD_ABILITY_KICK,
            Settings.VAL_PU_FIELD_EXTRA_SPEED,
            Settings.VAL_PU_FIELD_ABLITY_PUNCH,
            Settings.VAL_PU_FIELD_ABILITY_GRAB,
            Settings.VAL_PU_FIELD_SPOOGER,
            Settings.VAL_PU_FIELD_GOLDFLAME,
            Settings.VAL_PU_FIELD_TRIGGER,
            Settings.VAL_PU_FIELD_JELLY,
            Settings.VAL_PU_FIELD_EBOLA,
            Settings.VAL_PU_FIELD_RANDOM,
        };

        private FieldCellArray cells;
        private PlayerList players;
        private TimerManager timerManager;

        public Field()
        {
            currentField = this;
            timerManager = new TimerManager();
            players = new PlayerList();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Setup

        public void Setup(Scheme scheme)
        {
            SetupField(scheme.GetFieldData(), scheme.GetBrickDensity());
            SetupPlayers(players, scheme.GetPlayerLocations());
            SetupPowerups(scheme.GetPowerupInfo());
        }

        private void SetupField(FieldData data, int brickDensity)
        {
            int width = data.GetWidth();
            int height = data.GetHeight();

            cells = new FieldCellArray(width, height);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    FieldBlocks block = data.Get(x, y);
                    switch (block)
                    {
                        case FieldBlocks.Blank:
                        {   
                            break;
                        }

                        case FieldBlocks.Brick:
                        {
                            if (MathHelp.NextInt(100) <= brickDensity)
                            {
                                SetCell(new BrickCell(x, y));
                            }
                            break;
                        }

                        case FieldBlocks.Solid:
                        {
                            SetCell(new SolidCell(x, y));
                            break;
                        }

                        default:
                        {
                            Debug.Assert(false, "Unsupported cell type: " + block);
                            break;
                        }
                    }
                }
            }
        }

        private void SetupPlayers(PlayerList players, PlayerLocationInfo[] locations)
        {
            List<Player> playerList = players.list;
            foreach (Player player in playerList)
            {
                int index = player.GetIndex();
                PlayerLocationInfo info = locations[index];
                int cx = info.x;
                int cy = info.y;

                if (cx == 0 && cy == 0)
                {
                    SetCell(player);
                }
                else
                {
                    player.SetCell(info.x, info.y);
                }

                ClearCell(cx - 1, cy);
                ClearCell(cx, cy - 1);
                ClearCell(cx + 1, cy);
                ClearCell(cx, cy + 1);
            }
        }

        private void SetupPowerups(PowerupInfo[] powerupInfo)
        {
            foreach (PowerupInfo info in powerupInfo)
            {
                if (info.bornWith)
                {
                    List<Player> playerList = players.list;
                    foreach (Player player in playerList)
                    {
                        player.TryAddPowerup(info.powerupIndex);
                    }
                }
            }

            BrickCell[] brickCells = new BrickCell[GetWidth() * GetHeight()];
            int count = GetBrickCells(brickCells);
            if (count == 0)
            {
                return;
            }

            ShuffleCells(brickCells, count);

            int brickIndex = 0;
            foreach (PowerupInfo info in powerupInfo)
            {
                if (info.forbidden || info.bornWith)
                {
                    continue;
                }

                int powerupIndex = info.powerupIndex;
                int powerupCount = POWERUPS_COUNT[powerupIndex];
                if (powerupCount < 0)
                {
                    if (MathHelp.NextInt(10) < -powerupCount)
                    {
                        continue;
                    }
                    powerupCount = 1;
                }

                for (int i = 0; i < powerupCount; ++i)
                {
                    BrickCell cell = brickCells[brickIndex++];
                    int cx = cell.GetCx();
                    int cy = cell.GetCy();

                    cell.powerup = powerupIndex;

                    if (brickIndex == count)
                    {
                        break;
                    }
                }

                if (brickIndex == count)
                {
                    break;
                }
            }
        }

        private int GetBrickCells(BrickCell[] array)
        {
            int count = 0;
            FieldCell[] cellArray = cells.GetArray();
            foreach (FieldCell cell in cellArray)
            {
                if (cell != null)
                {
                    BrickCell brickCell = cell.AsBrick();
                    if (brickCell != null)
                    {
                        if (brickCell.powerup == Powerups.None)
                        {
                            array[count++] = brickCell;
                        }
                    }
                }
            }

            return count;
        }

        private void ShuffleCells(FieldCell[] array, int size)
        {
            Util.ShuffleArray(array, size);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////
        
        #region Updatable

        public void Update(float delta)
        {
            timerManager.Update(delta);
            UpdateCells(delta);
            HandleCollisions();
        }

        private void UpdateCells(float delta)
        {
            FieldCell[] cellsArray = cells.GetArray();
            for (int i = 0; i < cellsArray.Length; ++i)
            {
                FieldCell cell = cellsArray[i];
                if (cell != null)
                {
                    UpdateCell(delta, cell);
                }
            }
        }

        private void UpdateCell(float delta, FieldCell cell)
        {
            do 
            {
                Debug.Assert(Debug.flag && CheckCell(cell));
                cell.Update(delta);

                cell = cell.listNext;
            } 
            while (cell != null);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Players

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public PlayerList GetPlayers()
        {
            return players;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Bombs

        public void SetBomb(Bomb bomb)
        {
            SetCell(bomb);
        }

        /** Should only be called from Bomb.Blow() */
        public void BlowBomb(Bomb bomb)
        {
            int cx = bomb.GetCx();
            int cy = bomb.GetCy();
            SetExplosion(bomb, cx, cy);

            bomb.player.OnBombBlown(bomb);

            bool up = true, down = true, left = true, right = true;
            int radius = bomb.GetRadius();

            for (int i = 1; i <= radius && (up || down || left || right); ++i)
            {
                left = left && SetExplosion(bomb, cx - i, cy);
                up = up && SetExplosion(bomb, cx, cy - i);
                down = down && SetExplosion(bomb, cx, cy + i);
                right = right && SetExplosion(bomb, cx + i, cy);
            }
        }

        private bool SetExplosion(Bomb bomb, int cx, int cy)
        {
            FieldCell cell = GetCell(cx, cy);
            if (cell == null)
            {
                return false; // bomb hits the wall
            }

            if (cell.IsBrick())
            {
                BrickCell brick = (BrickCell)cell;
                if (!brick.destroyed)
                {
                    brick.Destroy();
                }
                return false; // bomb destroyed a brick
            }

            if (cell.IsSolid())
            {
                return false; // bomb hits solid block
            }

            if (cell.IsBomb() && cell != bomb)
            {
                cell.AsBomb().Blow();
                return true;
            }

            if (cell.IsPowerup())
            {
                cell.RemoveFromField();
                return false;
            }

            if (cell.IsBomb())
            {
                cell.RemoveFromField();
            }
            SetCell(new FlameCell(cx, cy));
            return true;
        }

        public void DestroyBrick(BrickCell brick)
        {
            brick.RemoveFromField();

            int powerup = brick.powerup;
            if (powerup != Powerups.None)
            {
                int cx = brick.GetCx();
                int cy = brick.GetCy();
                SetCell(new PowerupCell(powerup, cx, cy));
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Powerups

        public void PlacePowerup(int powerupIndex)
        {

        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Cells

        public void MoveablePosChanged(MovableCell movable)
        {
            HandleWallCollisions(movable);
        }

        public void MovableCellChanged(MovableCell movable, int oldCx, int oldCy)
        {
            SetCell(movable);
        }

        public void PlayerCellChanged(Player player, int oldCx, int oldCy)
        {
        }

        public FieldCell GetCell(int cx, int cy)
        {
            return cells.Get(cx, cy);
        }

        public void SetCell(FieldCell cell)
        {
            cells.Add(cell.GetCx(), cell.GetCy(), cell);
        }

        public void RemoveCell(FieldCell cell)
        {
            int cx = cell.GetCx();
            int cy = cell.GetCy();

            cells.Remove(cell);
        }

        private void ClearCell(int cx, int cy)
        {
            if (IsInsideField(cx, cy))
            {
                FieldCell cell = GetCell(cx, cy);
                if (cell != null && !cell.IsSolid())
                {
                    RemoveCell(cell);
                }
            }
        }

        public bool IsObstacleCell(int cx, int cy)
        {
            if (!IsInsideField(cx, cy))
            {
                return true;
            }

            FieldCell cell = GetCell(cx, cy);
            return cell != null && cell.IsObstacle();
        }

        public bool IsInsideField(int cx, int cy)
        {
            return cx >= 0 && cy >= 0 && cx < GetWidth() && cy < GetHeight();
        }

        private bool CheckCell(FieldCell cell)
        {
            return cells.Contains(cell);
        }

        public FieldCellArray GetCells()
        {
            return cells;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Collisions

        private void HandleCollisions()
        {
            FieldCell[] cellsArray = cells.GetArray();
            for (int i = 0; i < cellsArray.Length; ++i)
            {
                FieldCell cell = cellsArray[i];
                HandleCollisions(cell);
            }
        }

        private void HandleCollisions(FieldCell cell)
        {
            for (FieldCell c1 = cell; c1 != null; c1 = c1.listNext)
            {
                // check collisions with everyone from the same cell
                for (FieldCell c2 = c1.listNext; c2 != null; c2 = c2.listNext)
                {
                    HandleCollisions(c1, c2);
                }

                // collision optimization: check only for right and down
                int cx = c1.cx;
                int cy = c1.cy;

                HandleCollisions(c1, cx + 1, cy);
                HandleCollisions(c1, cx, cy + 1);
                HandleCollisions(c1, cx + 1, cy + 1);
            }
        }

        private void HandleCollisions(FieldCell cell, int neighborCx, int neighborCy)
        {
            if (IsInsideField(neighborCx, neighborCy))
            {
                FieldCell neighborCell = GetCell(neighborCx, neighborCy);
                for (FieldCell c = neighborCell; c != null; c = c.listNext)
                {
                    HandleCollisions(cell, c);
                }
            }
        }

        private void HandleCollisions(FieldCell c1, FieldCell c2)
        {
            if (Collides(c1, c2))
            {
                if (!c1.HandleCollision(c2))
                {
                    c2.HandleCollision(c1);
                }
            }
        }

        private void HandleWallCollisions(MovableCell movable)
        {   
            float dx = movable.moveDx;
            float dy = movable.moveDy;

            if (dx > 0)
            {
                float maxX = GetMaxPx();
                if (movable.GetPx() > maxX)
                {
                    movable.SetPosX(maxX);
                    movable.HandleWallCollision();
                }
            }
            else if (dx < 0)
            {
                float minX = GetMinPx();
                if (movable.GetPx() < minX)
                {
                    movable.SetPosX(minX);
                    movable.HandleWallCollision();
                }
            }

            if (dy > 0)
            {
                float maxY = GetMaxPy();
                if (movable.GetPy() > maxY)
                {
                    movable.SetPosY(maxY);
                    movable.HandleWallCollision();
                }
            }
            else if (dy < 0)
            {
                float minY = GetMinPy();
                if (movable.GetPy() < minY)
                {
                    movable.SetPosY(minY);
                    movable.HandleWallCollision();
                }
            }
        }

        private bool Collides(FieldCell a, FieldCell b)
        {
            return Math.Abs(a.px - b.px) < Constant.CELL_WIDTH && Math.Abs(a.py - b.py) < Constant.CELL_HEIGHT;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Getters/Setters

        public static Field Current()
        {
            return currentField;
        }

        public int GetWidth()
        {
            return cells.GetWidth();
        }

        public int GetHeight()
        {
            return cells.GetHeight();
        }

        public float GetMinPx()
        {
            return Constant.CELL_WIDTH_2;
        }

        public float GetMinPy()
        {
            return Constant.CELL_HEIGHT_2;
        }

        public float GetMaxPx()
        {
            return Constant.FIELD_WIDTH - Constant.CELL_WIDTH_2;
        }

        public float GetMaxPy()
        {
            return Constant.FIELD_HEIGHT - Constant.CELL_HEIGHT_2;
        }

        #endregion

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
    }
}

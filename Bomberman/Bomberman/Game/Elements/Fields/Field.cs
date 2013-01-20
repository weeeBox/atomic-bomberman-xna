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
        private FieldCellArray cells;

        private PlayerList players;

        private static Field currentField;

        private TimerManager timerManager;

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
        
        public Field(Scheme scheme, PlayerList players)
        {
            this.players = players;

            currentField = this;
            timerManager = new TimerManager();

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
                            cells.Set(x, y, new BlankCell(x, y));
                            break;
                        }

                        case FieldBlocks.Brick:
                        {
                            if (MathHelper.NextInt(100) <= brickDensity)
                            {
                                cells.Set(x, y, new BrickCell(x, y));
                            }
                            else
                            {
                                cells.Set(x, y, new BlankCell(x, y));
                            }
                            break;
                        }

                        case FieldBlocks.Solid:
                        {
                            cells.Set(x, y, new SolidCell(x, y));
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
                player.SetCell(info.x, info.y);

                int fromCx = Math.Max(0, cx - 1);
                int toCx = Math.Min(cx + 1, GetWidth() - 1);
                int fromCy = Math.Max(0, cy - 1);
                int toCy = Math.Min(cy + 1, GetHeight() - 1);

                for (int x = fromCx; x <= toCx; ++x)
                {
                    for (int y = fromCy; y <= toCy; ++y)
                    {
                        ClearCell(x, y);
                    }
                }
            }
        }

        private void SetupPowerups(PowerupInfo[] powerupInfo)
        {
            BrickCell[] brickCells = new BrickCell[GetWidth() * GetHeight()];
            int count = GetBrickCells(brickCells);
            if (count == 0)
            {
                return;
            }

            ShuffleArray(brickCells, count);

            int brickIndex = 0;
            foreach (PowerupInfo info in powerupInfo)
            {
                if (!info.forbidden)
                {
                    int powerupIndex = info.powerupIndex;
                    int powerupCount = POWERUPS_COUNT[powerupIndex];
                    if (powerupCount < 0)
                    {
                        if (MathHelper.NextInt(10) < -powerupCount)
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
        }

        private int GetEmptyCells(FieldCell[] array)
        {
            int count = 0;
            FieldCell[] cellArray = cells.GetArray();
            foreach (FieldCell cell in cellArray)
            {
                if (cell.IsEmpty() && !IsPlayerAt(cell.GetCx(), cell.GetCy()))
                {
                    cellArray[count++] = cell;
                }
            }

            return count;
        }

        private int GetBrickCells(BrickCell[] array)
        {
            int count = 0;
            FieldCell[] cellArray = cells.GetArray();
            foreach (FieldCell cell in cellArray)
            {
                if (cell.IsBrick() && !IsPlayerAt(cell.GetCx(), cell.GetCy()))
                {
                    BrickCell brickCell = (BrickCell)cell;
                    if (brickCell.powerup == Powerups.None)
                    {
                        array[count++] = (BrickCell)cell;
                    }
                }
            }

            return count;
        }

        private bool IsPlayerAt(int cx, int cy)
        {
            return GetPlayer(cx, cy) != null;
        }

        private Player GetPlayer(int cx, int cy)
        {
            foreach (Player player in players.list)
            {
                if (player.GetCx() == cx && player.GetCy() == cy)
                {
                    return player;
                }
            }

            return null;
        }

        private void ShuffleArray(FieldCell[] array, int size)
        {   
            int n = size;
            while (n > 1)
            {
                n--;
                int k = MathHelper.NextInt(n + 1);
                FieldCell value = array[k];
                array[k] = array[n];
                array[n] = value;
            } 
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

        public void DestroyBrick(BrickCell brick)
        {
            int cx = brick.GetCx();
            int cy = brick.GetCy();
            int powerup = brick.powerup;
            if (powerup != Powerups.None)
            {
                SetCell(new PowerupCell(powerup, cx, cy));
            }
            else
            {
                ClearCell(cx, cy);
            }
        }

        public void PlacePowerup(int powerupIndex)
        {
            
        }

        public void BlowBomb(Bomb bomb)
        {
            int cx = bomb.GetCx();
            int cy = bomb.GetCy();

            bomb.Blow();

            SetExplosion(bomb, cx, cy);
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
                bool spreaded = SetExplosion(bomb, cx, cy);
                if (!spreaded)
                {
                    break;
                }

                cx += dcx;
                cy += dcy;
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
                BlowBomb((Bomb)cell);
                return true;
            }

            SetCell(new ExplosionCell(cx, cy));
            return true;
        }

        public PlayerList GetPlayers()
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
            FieldCell cell = GetCell(cx, cy);
            if (cell != null && !cell.IsEmpty() && !cell.IsSolid())
            {
                SetCell(new BlankCell(cx, cy));
            }
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

        private void ProcessCollision(MovableCell movable, int cx, int cy)
        {
            if (cx >= 0 && cx < GetWidth() && cy >=0 && cy < GetHeight())
            {
                FieldCell cell = cells.Get(cx, cy);

                if (cell.IsObstacle())
                {
                    if (Collides(movable, cell))
                    {
                        bool bombKicked = TryKickBomb(movable, cell);
                        if (!bombKicked)
                        {
                            AdjustPosition(movable, cx, cy);
                        }
                    }
                }
                else if (cell.IsPowerup())
                {
                    if (Collides(movable, cell))
                    {
                        if (movable.IsPlayer())
                        {
                            PowerupCell powerupCell = (PowerupCell)cell;
                            int powerup = powerupCell.powerup;

                            Player player = (Player)movable;
                            player.TryAddPowerup(powerup);
                        }
                        ClearCell(cx, cy);
                    }
                }
            }
        }

        private bool TryKickBomb(MovableCell movable, FieldCell cell)
        {
            if (movable.IsPlayer() && cell.IsBomb())
            {
                Player player = (Player)movable;
                if (!player.CanKick())
                {
                    return false;
                }

                Direction direction = player.GetDirection();
                FieldCell blockingCell = cell.NearCellDir(direction);
                if (blockingCell == null || blockingCell.IsObstacle()) // can we kick the bomb here?
                {
                    return false;
                }

                Bomb bomb = (Bomb)cell;
                bomb.Kick(direction);

                return true;
            }

            return false;
        }

        private static void AdjustPosition(MovableCell movable, int cx, int cy)
        {
            switch (movable.GetDirection())
            {
                case Direction.UP:
                    {
                        movable.SetPosY(Util.Cy2Py(cy + 1));
                        break;
                    }
                case Direction.DOWN:
                    {
                        movable.SetPosY(Util.Cy2Py(cy - 1));
                        break;
                    }
                case Direction.LEFT:
                    {
                        movable.SetPosX(Util.Cx2Px(cx + 1));
                        break;
                    }
                case Direction.RIGHT:
                    {
                        movable.SetPosX(Util.Cx2Px(cx - 1));
                        break;
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

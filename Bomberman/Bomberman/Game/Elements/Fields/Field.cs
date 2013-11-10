using System;
using System.Collections.Generic;
using BomberEngine;
using Bomberman.Content;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Players;
using BombermanCommon.Resources.Scheme;

namespace Bomberman.Game.Elements.Fields
{
    public class Field : BaseObject, IUpdatable, IDestroyable, IResettable
    {
        private static Field currentField;

        private Game m_game;

        private FieldCellArray m_cells;

        private LinkedList<MovableCell> m_movableCells;
        
        private LinkedList<FieldCell> m_tempCellsList;
        private List<MovableCell> m_tempMovableList;

        private PlayerAnimations m_playerAnimations;
        private BombAnimations m_bombAnimations;

        private List<Bomb> m_detonatedBombs;
        private bool m_detonating;

        public Field(Game game)
        {
            m_game = game;

            currentField = this;
            m_tempCellsList = new LinkedList<FieldCell>();
            m_tempMovableList = new List<MovableCell>();

            m_movableCells = new LinkedList<MovableCell>();

            m_playerAnimations = new PlayerAnimations();
            m_bombAnimations = new BombAnimations();

            m_detonatedBombs = new List<Bomb>(16);
        }

        /* mock constructor */
        internal Field(int width, int height)
        {
            currentField = this;
            m_cells = new FieldCellArray(width, height);

            m_tempCellsList = new LinkedList<FieldCell>();
            m_tempMovableList = new List<MovableCell>();

            m_movableCells = new LinkedList<MovableCell>();
        }

        public void Reset()
        {
            timerManager.CancelAll();

            m_cells.Reset();
            players.Reset();
            
            m_movableCells.Clear();
            
            m_tempCellsList.Clear();
            m_tempMovableList.Clear();
        }

        public void Destroy()
        {
            timerManager.CancelAll();
            currentField = null;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Setup

        public void Load(Scheme scheme)
        {
            SetupField(scheme.GetFieldData(), scheme.GetBrickDensity());
            SetupPlayers(players, scheme.GetPlayerLocations());
            SetupPowerups(scheme.GetPowerupInfo());
        }

        public void Setup(Scheme scheme)
        {
            SetupField(scheme.GetFieldData());
        }

        public void Restart(Scheme scheme)
        {
            Reset();
            Load(scheme); // TODO: reuse objects
        }

        private void SetupField(FieldData data, int brickDensity = -1)
        {
            int width = data.GetWidth();
            int height = data.GetHeight();

            m_cells = new FieldCellArray(width, height);

            m_movableCells = new LinkedList<MovableCell>();

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
                            if (brickDensity == -1 || MathHelp.NextInt(100) <= brickDensity)
                            {
                                AddCell(new BrickCell(x, y));
                            }
                            break;
                        }

                        case FieldBlocks.Solid:
                        {
                            AddCell(new SolidCell(x, y));
                            break;
                        }

                        default:
                        {
                            Assert.IsTrue(false, "Unsupported cell type: " + block);
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
                    AddCell(player);
                }
                else
                {
                    player.SetCell(info.x, info.y);
                }

                ClearBrick(cx - 1, cy);
                ClearBrick(cx, cy - 1);
                ClearBrick(cx + 1, cy);
                ClearBrick(cx, cy + 1);
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

            CVar[] POWERUPS_COUNT = CVars.powerupsCount;

            int brickIndex = 0;
            foreach (PowerupInfo info in powerupInfo)
            {
                if (info.forbidden || info.bornWith)
                {
                    continue;
                }

                int powerupIndex = info.powerupIndex;
                int powerupCount = POWERUPS_COUNT[powerupIndex].intValue;
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
            FieldCellSlot[] slots = m_cells.slots;
            foreach (FieldCellSlot slot in slots)
            {   
                BrickCell brickCell = slot.GetBrick();
                if (brickCell != null)
                {
                    if (brickCell.powerup == Powerups.None)
                    {
                        array[count++] = brickCell;
                    }
                }
            }

            return count;
        }

        private int GetEmptySlots(FieldCellSlot[] array)
        {
            int count = 0;
            FieldCellSlot[] slots = m_cells.slots;
            for (int i = 0; i < slots.Length; ++i)
            {
                FieldCellSlot slot = slots[i];
                if (slot.IsEmpty())
                {
                    array[count++] = slot;
                }
            }

            return count;
        }

        private void ShuffleCells(FieldCell[] array, int size)
        {
            ArrayUtils.Shuffle(array, size);
        }

        private void ShuffleSlots(FieldCellSlot[] array, int size)
        {
            ArrayUtils.Shuffle(array, size);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////
        
        #region Updatable

        public void Update(float delta)
        {
            timerManager.Update(delta);
            
            UpdateCells(delta);
            UpdatePhysics(delta);
        }

        private void UpdateCells(float delta)
        {
            FieldCellSlot[] slots = m_cells.slots;
            foreach (FieldCellSlot slot in slots)
            {
                UpdateSlot(delta, slot);
            }
        }

        public void UpdateSlot(float delta, FieldCellSlot slot)
        {
            slot.GetCells(m_tempCellsList);

            if (m_tempCellsList.Count > 0)
            {
                foreach (FieldCell cell in m_tempCellsList)
                {
                    cell.Update(delta);
                }
                m_tempCellsList.Clear();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Physics

        private void UpdatePhysics(float delta)
        {
            UpdateMoving(delta);
            HandleCollisions();
        }

        private void UpdateMoving(float delta)
        {
            Assert.IsTrue(m_tempMovableList.Count == 0);
            foreach (MovableCell movable in m_movableCells)
            {
                if (movable.IsMoving())
                {
                    m_tempMovableList.Add(movable);
                }
            }
            
            for (int i = 0; i < m_tempMovableList.Count; ++i)
            {
                m_tempMovableList[i].UpdateMoving(delta);
            }
            m_tempMovableList.Clear();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Round

        private void ScheduleEndRound()
        {
            ScheduleTimer(EndRound, CVars.winDelay.floatValue);
        }

        private void ScheduleRoundEndCheck()
        {
            ScheduleTimerOnce(CheckEndRound);
        }

        private void CheckEndRound()
        {
            if (players.GetAlivePlayerCount() < 2)
            {
                ScheduleEndRound();
            }
        }

        private void EndRound()
        {
            // find a winner
            foreach (Player player in players.list)
            {
                if (player.IsAlive)
                {
                    player.statistics.winsCount++;
                    break;
                }
            }

            GetGame().EndRound();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Players

        public void AddPlayer(Player player)
        {
            players.Add(player);
            player.animations = m_playerAnimations;
            player.bombAnimations = m_bombAnimations;
        }

        public void RemovePlayer(Player player)
        {
            players.Remove(player);
            player.animations = null;
            player.bombAnimations = null;
            CancelAllTimers(player);
        }

        public void KillPlayer(Player victim, Player killer)
        {
            if (victim.IsAlive && !CVars.c_noKills.boolValue)
            {
                players.Kill(victim);

                // update score points
                if (victim == killer) // dumb idiot kills himself?
                {
                    victim.statistics.suicidesCount++;
                }
                else
                {
                    killer.statistics.killsCount++;
                }
  
                DropPowerups(victim);
                ScheduleRoundEndCheck();
            }
        }

        public PlayerList GetPlayers()
        {
            return players;
        }

        public void DropPowerups(Player player)
        {
            FieldCellSlot[] slots = new FieldCellSlot[GetWidth() * GetHeight()];
            int slotsCount = GetEmptySlots(slots);
            if (slotsCount == 0)
            {
                return;
            }

            ShuffleSlots(slots, slotsCount);

            PowerupList powerups = player.powerups;
            int slotIndex = 0;
            for (int powerupIndex = 0; powerupIndex < Powerups.Count && slotIndex < slots.Length; ++powerupIndex)
            {
                int count = powerups.GetCount(powerupIndex);
                for (int i = 0; i < count && slotIndex < slots.Length; ++i)
                {
                    FieldCellSlot slot = slots[slotIndex++];
                    PowerupCell cell = new PowerupCell(powerupIndex, slot.cx, slot.cy);
                    AddCell(cell);
                }   
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Bombs

        public void SetBomb(Bomb bomb)
        {
            AddCell(bomb);

            FieldCellSlot slot = GetSlot(bomb);
            PowerupCell powerup = slot.GetPowerup();
            if (powerup != null)
            {
                powerup.RemoveFromField();
            }
            else
            {
                FlameCell flame = slot.GetFlame();
                if (flame != null)
                {
                    bomb.Blow();
                }
            }
        }

        /** Should only be called from Bomb.Blow() */
        public void BlowBomb(Bomb bomb)
        {
            int cx = bomb.GetCx();
            int cy = bomb.GetCy();

            bomb.RemoveFromField();
            SetFlame(bomb, cx, cy);

            bomb.player.OnBombBlown(bomb);

            bool up = true, down = true, left = true, right = true;
            int radius = bomb.GetRadius();

            for (int i = 1; i <= radius && (up || down || left || right); ++i)
            {
                left = left && SetFlame(bomb, cx - i, cy);
                up = up && SetFlame(bomb, cx, cy - i);
                down = down && SetFlame(bomb, cx, cy + i);
                right = right && SetFlame(bomb, cx + i, cy);
            }

            if (m_detonatedBombs.Count > 0 && !m_detonating)
            {
                m_detonating = true;

                for (int i = 0; i < m_detonatedBombs.Count; ++i)
                {
                    m_detonatedBombs[i].Blow();
                }

                m_detonating = false;
                m_detonatedBombs.Clear();
            }
        }

        /* Returns true if can be spread more */
        private bool SetFlame(Bomb bomb, int cx, int cy)
        {
            if (!IsInsideField(cx, cy))
            {
                return false;
            }

            FieldCellSlot slot = GetSlot(cx, cy);

            FieldCell staticCell = slot.staticCell;
            if (staticCell != null)
            {
                if (staticCell.IsSolid())
                {
                    return false;
                }

                if (staticCell.IsBrick())
                {
                    BrickCell brick = staticCell.AsBrick();
                    if (!brick.hit)
                    {
                        brick.Hit();
                    }

                    return false;
                }

                if (staticCell.IsPowerup())
                {
                    staticCell.AsPowerup().RemoveFromField();
                    return false;
                }
            }

            if (slot.MovableCount() > 0)
            {
                LinkedList<FieldCell> tempList = new LinkedList<FieldCell>();
                slot.GetCells(tempList);

                foreach (FieldCell cell in tempList)
                {
                    if (cell.IsBomb())
                    {
                        m_detonatedBombs.Add(cell.AsBomb()); // I'll blow you later
                    }
                    else if (cell.IsPlayer())
                    {
                        KillPlayer(cell.AsPlayer(), bomb.player);
                    }
                }
            }

            SetFlameHelper(bomb, cx, cy);
            return true;
        }

        private void SetFlameHelper(Bomb bomb, int cx, int cy)
        {
            Player player = bomb.player;
            Assert.IsNotNull(player);

            FieldCellSlot slot = GetSlot(cx, cy);
            FlameCell flame = slot.GetFlame();
            if (flame != null && flame.player == player)
            {
                flame.RemoveFromField();
            }

            // create flame
            int dx = cx - bomb.cx;
            int dy = cy - bomb.cy;
            
            flame = new FlameCell(player, cx, cy);

            // center?
            flame.isCenter = dx == 0 && dy == 0;
            if (!flame.isCenter)
            {
                // direction?
                flame.direction = dx > 0 ? Direction.RIGHT :
                    (dx < 0 ? Direction.LEFT :
                    (dy > 0 ? Direction.DOWN : Direction.UP));

                // short?
                flame.isShort = bomb.isShort;
                if (flame.isShort)
                {
                    flame.isCap = true;
                }
                else
                {
                    // cap?
                    int radius = bomb.GetRadius();
                    flame.isCap = Math.Abs(dx) == radius || Math.Abs(dy) == radius;
                }
            }

            AddCell(flame);
        }

        public void DestroyBrick(BrickCell brick)
        {
            brick.RemoveFromField();

            int powerup = brick.powerup;
            if (powerup != Powerups.None)
            {
                int cx = brick.GetCx();
                int cy = brick.GetCy();
                AddCell(new PowerupCell(powerup, cx, cy));
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

        public void MovableCellChanged(MovableCell movable, int oldCx, int oldCy)
        {
            AddCell(movable);
        }

        public FieldCellSlot GetSlot(int index)
        {
            FieldCellSlot[] slots = m_cells.slots;
            return slots[index];
        }

        public FieldCellSlot GetSlot(FieldCell cell)
        {   
            return GetSlot(cell.cx, cell.cy);
        }

        public FieldCellSlot GetSlot(int cx, int cy)
        {
            return IsInsideField(cx, cy) ? m_cells.Get(cx, cy) : null;
        }

        public void AddCell(FieldCell cell)
        {
            m_cells.Add(cell.GetCx(), cell.GetCy(), cell);
            if (cell.IsMovable())
            {
                AddMovable(cell.AsMovable());
            }
        }

        public void RemoveCell(FieldCell cell)
        {   
            m_cells.Remove(cell);
            if (cell.IsMovable())
            {
                RemoveMovable(cell.AsMovable());
            }
            CancelAllTimers(cell);
        }

        private void ClearBrick(int cx, int cy)
        {
            if (IsInsideField(cx, cy))
            {
                BrickCell brick = GetBrick(cx, cy);
                if (brick != null)
                {   
                    RemoveCell(brick);
                }
            }
        }

        private void AddMovable(MovableCell cell)
        {
            if (!cell.addedToList)
            {
                cell.addedToList = true;

                // added into the list sorted bombs first, players next
                for (LinkedListNode<MovableCell> node = m_movableCells.First; node != null; node = node.Next)
                {
                    if (cell.type <= node.Value.type)
                    {
                        m_movableCells.AddBefore(node, cell);
                        return;
                    }
                }
                m_movableCells.AddLast(cell);
            }
        }

        private void RemoveMovable(MovableCell cell)
        {
            if (cell.addedToList)
            {
                m_movableCells.Remove(cell);
                cell.addedToList = false;
            }
        }

        public bool IsObstacleCell(int cx, int cy)
        {
            if (IsInsideField(cx, cy))
            {
                return GetSlot(cx, cy).ContainsObstacle();
            }

            return true;
        }

        public bool IsInsideField(int cx, int cy)
        {
            return cx >= 0 && cy >= 0 && cx < GetWidth() && cy < GetHeight();
        }

        private bool CheckCell(FieldCell cell)
        {
            return m_cells.Contains(cell);
        }

        public FieldCellArray GetCells()
        {
            return m_cells;
        }

        public FieldCellSlot[] GetSlots()
        {
            return m_cells.slots;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public SolidCell GetSolid(int cx, int cy)
        {
            FieldCellSlot slot = GetSlot(cx, cy);
            return slot != null ? slot.GetSolid() : null;
        }

        public BrickCell GetBrick(int cx, int cy)
        {
            FieldCellSlot slot = GetSlot(cx, cy);
            return slot != null ? slot.GetBrick() : null;
        }

        public PowerupCell GetPowerup(int cx, int cy)
        {
            FieldCellSlot slot = GetSlot(cx, cy);
            return slot != null ? slot.GetPowerup() : null;
        }

        public FlameCell GetFlame(int cx, int cy)
        {
            FieldCellSlot slot = GetSlot(cx, cy);
            return slot != null ? slot.GetFlame() : null;
        }

        public Bomb GetBomb(int cx, int cy)
        {
            FieldCellSlot slot = GetSlot(cx, cy);
            return slot != null ? slot.GetBomb() : null;
        }

        public bool ContainsNoObstacle(int cx, int cy)
        {
            FieldCellSlot slot = GetSlot(cx, cy);
            return slot != null && !slot.ContainsObstacle();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Collisions

        private void HandleCollisions()
        {
            // copy to the temp list
            Assert.IsTrue(m_tempMovableList.Count == 0);
            foreach (MovableCell movable in m_movableCells)
            {   
                m_tempMovableList.Add(movable);
            }

            // static collisions
            for (int i = 0; i < m_tempMovableList.Count; ++i)
            {
                CheckStaticCollisions(m_tempMovableList[i]);
            }
            m_tempMovableList.Clear();

            // copy to the temp list
            foreach (MovableCell movable in m_movableCells)
            {
                m_tempMovableList.Add(movable);
            }

            // movable-to-movable collisions
            for (int i = 0; i < m_tempMovableList.Count - 1; ++i)
            {
                MovableCell c1 = m_tempMovableList[i];
                for (int j = i + 1; j < m_tempMovableList.Count; ++j)
                {
                    MovableCell c2 = m_tempMovableList[j];
                    CheckCollision(c1, c2);
                }
            }
            
            // clear
            m_tempMovableList.Clear();
        }

        public bool CheckStaticCollisions(MovableCell movable)
        {
            bool hasCollision = false;

            hasCollision |= CheckWallCollisions(movable);
            hasCollision |= CheckStillCellsCollisions(movable);

            return hasCollision;
        }

        /* Checks collision with static cells (bricks and solids) + still movable obstacles (not moving bombs) */
        private bool CheckStillCellsCollisions(MovableCell movable)
        {
            int cx = movable.cx;
            int cy = movable.cy;

            int stepX = Math.Sign(movable.px - movable.CellCenterPx());
            int stepY = Math.Sign(movable.py - movable.CellCenterPy());

            bool hasStepX = stepX != 0;
            bool hasStepY = stepY != 0;

            bool hasCollision = false;

            hasCollision |= CheckStillCollisions(movable, GetSlot(cx, cy));

            if (hasStepX && hasStepY)
            {
                hasCollision |= CheckStillCollisions(movable, GetSlot(cx + stepX, cy));
                hasCollision |= CheckStillCollisions(movable, GetSlot(cx, cy + stepY));
                hasCollision |= CheckStillCollisions(movable, GetSlot(cx + stepX, cy + stepY));
            }
            else if (hasStepX)
            {
                hasCollision |= CheckStillCollisions(movable, GetSlot(cx + stepX, cy));
            }
            else if (hasStepY)
            {
                hasCollision |= CheckStillCollisions(movable, GetSlot(cx, cy + stepY));
            }

            return hasCollision;
        }

        private bool CheckStillCollisions(MovableCell movable, FieldCellSlot slot)
        {
            if (slot != null)
            {
                FieldCell staticCell = slot.staticCell;
                if (staticCell != null)
                {
                    return CheckCollision(movable, staticCell);
                }

                bool handled = false;

                List<MovableCell> movableCells = slot.movableCells;
                for (int i = 0; i < movableCells.Count; ++i)
                {
                    MovableCell other = movableCells[i];
                    if (!other.IsMoving() && movable != other)
                    {
                        handled |= CheckCollision(movable, other);
                    }
                }

                return handled;
            }

            return false;
        }

        private bool CheckWallCollisions(MovableCell movable)
        {   
            float minX = GetMinPx();
            float maxX = GetMaxPx();

            if (movable.GetPx() > maxX)
            {
                movable.SetPosX(maxX);
                movable.HandleWallCollision();

                return true;
            }

            if (movable.GetPx() < minX)
            {
                movable.SetPosX(minX);
                movable.HandleWallCollision();

                return true;
            }
            
            float minY = GetMinPy();
            float maxY = GetMaxPy();

            if (movable.GetPy() > maxY)
            {
                movable.SetPosY(maxY);
                movable.HandleWallCollision();
                
                return true;
            }

            if (movable.GetPy() < minY)
            {
                movable.SetPosY(minY);
                movable.HandleWallCollision();

                return true;
            }
            
            return false;
        }

        private bool CheckCollision(MovableCell c1, FieldCell c2)
        {
            return c1.Collides(c2) && c1.HandleCollision(c2);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Getters/Setters

        public static Field Current()
        {
            return currentField;
        }

        public static PlayerList Players
        {
            get { return currentField.players; }
        }

        public static List<Player> PlayersList
        {
            get { return Players.list; }
        }

        public static Player GetPlayer(int index)
        {
            return PlayersList[index];
        }

        public int GetWidth()
        {
            return m_cells.GetWidth();
        }

        public int GetHeight()
        {
            return m_cells.GetHeight();
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

        private Game GetGame()
        {
            return m_game;
        }

        public PlayerList players
        {
            get { return m_game.GetPlayers(); }
        }

        public TimerManager timerManager
        {
            get { return m_game.timerManager; }
        }

        public bool IsGameMuliplayerClient
        {
            get { return m_game.IsMuliplayerClient; }
        }

        public bool IsGameMuliplayerServer
        {
            get { return m_game.IsMuliplayerServer; }
        }

        public bool IsGameNetworkMultiplayer
        {
            get { return m_game.IsNetworkMultiplayer; }
        }

        public bool IsGameLocal
        {
            get { return m_game.IsLocal; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public void RunTest1()
        {
        }

        public void RunTest2()
        {
        }

        public void RunTest3()
        {
        }

        public void RunTest4()
        {
        }

        public void RunTest5()
        {
        }

        //////////////////////////////////////////////////////////////////////////////

        #region TimerManager

        public Timer ScheduleTimer(TimerCallback1 callback, float delay = 0.0f, bool repeated = false)
        {
            return timerManager.Schedule(callback, delay, repeated);
        }

        public Timer ScheduleTimer(TimerCallback2 callback, float delay = 0.0f, bool repeated = false)
        {
            return timerManager.Schedule(callback, delay, repeated);
        }

        public Timer ScheduleTimerOnce(TimerCallback1 callback, float delay = 0.0f, bool repeated = false)
        {
            return timerManager.ScheduleOnce(callback, delay, repeated);
        }

        public Timer ScheduleTimerOnce(TimerCallback2 callback, float delay = 0.0f, bool repeated = false)
        {
            return timerManager.ScheduleOnce(callback, delay, repeated);
        }

        public void CancelTimer(TimerCallback1 callback)
        {
            timerManager.Cancel(callback);
        }

        public void CancelTimer(TimerCallback2 callback)
        {
            timerManager.Cancel(callback);
        }

        public void CancelAllTimers(Object target)
        {
            timerManager.CancelAll(target);
        }

        public void CancelAllTimers()
        {
            timerManager.CancelAll();
        }

        #endregion
    }
}

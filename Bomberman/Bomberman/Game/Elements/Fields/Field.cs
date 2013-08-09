﻿using System;
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
using BomberEngine.Consoles;
using BomberEngine.Core.Events;

namespace Bomberman.Game.Elements.Fields
{
    public class Field : BaseObject, IUpdatable, IDestroyable, IResettable
    {
        private static Field currentField;

        private Game m_game;

        private FieldCellArray cells;
        private PlayerList players;
        private TimerManager timerManager;

        private LinkedList<MovableCell> movableCells;
        private LinkedList<CellContactList> contacts;

        private LinkedList<FieldCell> m_tempCellsList;
        private LinkedList<MovableCell> tempMoveList;

        private PlayerAnimations m_playerAnimations;
        private BombAnimations m_bombAnimations;

        public Field(Game game)
        {
            m_game = game;

            currentField = this;
            timerManager = new TimerManager();
            players = new PlayerList(timerManager, CVars.cg_maxPlayers.intValue);

            m_tempCellsList = new LinkedList<FieldCell>();
            tempMoveList = new LinkedList<MovableCell>();

            movableCells = new LinkedList<MovableCell>();
            contacts = new LinkedList<CellContactList>();

            m_playerAnimations = new PlayerAnimations();
            m_bombAnimations = new BombAnimations();
        }

        public void Reset()
        {
            timerManager.CancelAll();

            cells.Reset();
            players.Reset();
            
            movableCells.Clear();
            contacts.Clear();
            
            m_tempCellsList.Clear();
            tempMoveList.Clear();
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

            cells = new FieldCellArray(width, height);

            movableCells = new LinkedList<MovableCell>();

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
            FieldCellSlot[] slots = cells.slots;
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
            FieldCellSlot[] slots = cells.slots;
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

            if (IsGameDumbMuliplayerClient)
            {
                UpdateAnimations(delta);
                // the server sends game state: no need to calculate it
            }
            else
            {
                UpdateCells(delta);
                UpdatePhysics(delta);
            }
        }

        private void UpdateAnimations(float delta)
        {
            FieldCellSlot[] slots = cells.slots;
            foreach (FieldCellSlot slot in slots)
            {
                slot.GetCells(m_tempCellsList);
            }

            if (m_tempCellsList.Count > 0)
            {
                foreach (FieldCell cell in m_tempCellsList)
                {
                    cell.Update(delta);
                }
                m_tempCellsList.Clear();
            }
        }

        private void UpdateCells(float delta)
        {
            FieldCellSlot[] slots = cells.slots;
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
            CheckContacts();
        }

        private void UpdateMoving(float delta)
        {
            foreach (MovableCell movable in movableCells)
            {
                if (movable.IsMoving())
                {
                    tempMoveList.AddLast(movable);
                }
            }

            if (tempMoveList.Count > 0)
            {
                foreach (MovableCell movable in tempMoveList)
                {
                    UpdateMoving(delta, movable);
                }
                tempMoveList.Clear();
            }
        }

        private void UpdateMoving(float delta, MovableCell cell)
        {
            cell.UpdateMoving(delta);

            CheckWallCollisions(cell);
            CheckStaticCollisions(cell);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Round

        private void EndRound()
        {
            ScheduleTimer(RoundEndTimerCallback, CVars.winDelay.floatValue);
        }

        private void ScheduleRoundEndCheck()
        {
            ScheduleTimerOnce(RoundEndCheckTimerCallback);
        }

        private void RoundEndCheckTimerCallback(Timer timer)
        {
            if (players.GetAlivePlayerCount() < 2)
            {
                EndRound();
            }
        }

        private void RoundEndTimerCallback(Timer timer)
        {
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

        public void KillPlayer(Player player)
        {
            if (player.IsAlive() && !CVars.c_noKills.boolValue)
            {
                players.Kill(player);

                if (!IsGameDumbMuliplayerClient)
                {   
                    DropPowerups(player);
                }

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
                    if (!brick.destroyed)
                    {
                        brick.Destroy();
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
                        cell.AsBomb().Blow();
                    }
                    else if (cell.IsPlayer())
                    {
                        KillPlayer(cell.AsPlayer());
                    }
                }
            }

            SetFlame(bomb.player, cx, cy);
            return true;
        }

        private void SetFlame(Player player, int cx, int cy)
        {
            FieldCellSlot slot = GetSlot(cx, cy);
            FlameCell flame = slot.GetFlame();
            if (flame != null && flame.player == player)
            {
                flame.RemoveFromField();
            }
            AddCell(new FlameCell(player, cx, cy));
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
            FieldCellSlot[] slots = cells.slots;
            return slots[index];
        }

        public FieldCellSlot GetSlot(FieldCell cell)
        {   
            return GetSlot(cell.cx, cell.cy);
        }

        public FieldCellSlot GetSlot(int cx, int cy)
        {
            return IsInsideField(cx, cy) ? cells.Get(cx, cy) : null;
        }

        public void AddCell(FieldCell cell)
        {
            cells.Add(cell.GetCx(), cell.GetCy(), cell);
            if (cell.IsMovable())
            {
                AddMovable(cell.AsMovable());
            }
        }

        public void RemoveCell(FieldCell cell)
        {   
            cells.Remove(cell);
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
                for (LinkedListNode<MovableCell> node = movableCells.First; node != null; node = node.Next)
                {
                    if (cell.type <= node.Value.type)
                    {
                        movableCells.AddBefore(node, cell);
                        return;
                    }
                }
                movableCells.AddLast(cell);
            }
        }

        private void RemoveMovable(MovableCell cell)
        {
            if (cell.addedToList)
            {
                movableCells.Remove(cell);
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
            return cells.Contains(cell);
        }

        public FieldCellArray GetCells()
        {
            return cells;
        }

        public FieldCellSlot[] GetSlots()
        {
            return cells.slots;
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

        private void CheckContacts()
        {
            for (LinkedListNode<MovableCell> n1 = movableCells.First; n1 != null; n1 = n1.Next)
            {
                MovableCell c1 = n1.Value;
                int numContacts = 0;
                CellContactList cellContactList = c1.contactList;

                for (LinkedListNode<MovableCell> n2 = n1.Next; n2 != null; n2 = n2.Next)
                {
                    MovableCell c2 = n2.Value;
                    if (Collides(c1, c2))
                    {
                        cellContactList.Add(c2);
                        ++numContacts;
                    }
                }

                if (numContacts > 0)
                {
                    contacts.AddLast(cellContactList);
                }
            }

            if (contacts.Count > 0)
            {
                for (LinkedListNode<CellContactList> node = contacts.First; node != null; node = node.Next)
                {
                    CellContactList contactList = node.Value;
                    HandleContacts(contactList);
                    contactList.Clear();
                }
                contacts.Clear();
            }
        }

        private void HandleContacts(CellContactList contactList)
        {
            MovableCell targetCell = contactList.targetCell;
            LinkedList<MovableCell> cells = contactList.cells;
            for (LinkedListNode<MovableCell> node = cells.First; node != null; node = node.Next)
            {
                targetCell.HandleCollision(node.Value);
            }
        }

        private void CheckStaticCollisions(MovableCell movable)
        {
            int cx = movable.cx;
            int cy = movable.cy;

            int stepX = Math.Sign(movable.px - movable.CellCenterPx());
            int stepY = Math.Sign(movable.py - movable.CellCenterPy());

            bool hasStepX = stepX != 0;
            bool hasStepY = stepY != 0;

            CheckStaticCollisions(movable, GetSlot(cx, cy));

            if (hasStepX && hasStepY)
            {
                CheckStaticCollisions(movable, GetSlot(cx + stepX, cy));
                CheckStaticCollisions(movable, GetSlot(cx, cy + stepY));
                CheckStaticCollisions(movable, GetSlot(cx + stepX, cy + stepY));
            }
            else if (hasStepX)
            {
                CheckStaticCollisions(movable, GetSlot(cx + stepX, cy));
            }
            else if (hasStepY)
            {
                CheckStaticCollisions(movable, GetSlot(cx, cy + stepY));
            }
        }

        private void CheckStaticCollisions(MovableCell movable, FieldCellSlot slot)
        {
            if (slot != null)
            {
                FieldCell staticCell = slot.staticCell;
                if (staticCell != null)
                {
                    CheckCollision(movable, staticCell);
                }
                else
                {
                    FlameCell flame = slot.GetFlame();
                    if (flame != null && flame.GetCx() == slot.cx && flame.GetCy() == slot.cy)
                    {
                        movable.HandleCollision(flame);
                    }
                }
            }
        }

        private bool CheckCollision(MovableCell c1, FieldCell c2)
        {
            return Collides(c1, c2) && c1.HandleCollision(c2);
        }

        private void CheckWallCollisions(MovableCell movable)
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

        private Game GetGame()
        {
            return m_game;
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

        public bool IsGameDumbMuliplayerClient
        {
            get { return m_game.IsMuliplayerClient && CVars.sv_dumbClient.boolValue; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region TimerManager

        public Timer ScheduleTimer(TimerCallback callback, float delay = 0.0f, bool repeated = false)
        {
            return timerManager.Schedule(callback, delay, repeated);
        }

        public Timer ScheduleTimerOnce(TimerCallback callback, float delay = 0.0f, bool repeated = false)
        {
            return timerManager.ScheduleOnce(callback, delay, repeated);
        }

        public void CancelTimer(TimerCallback callback)
        {
            timerManager.Cancel(callback);
        }

        public void CancelAllTimers(Object target)
        {
            timerManager.CancelAll(target);
        }

        #endregion
    }
}

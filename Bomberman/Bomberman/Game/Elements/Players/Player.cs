using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Items;
using Bomberman.Game.Elements.Players.Input;
using Bomberman.Game.Elements.Fields;
using BomberEngine.Debugging;
using BomberEngine.Util;
using BomberEngine.Consoles;
using Lidgren.Network;
using BomberEngine.Game;
using BomberEngine.Core;

namespace Bomberman.Game.Elements.Players
{
    public class Player : MovableCell
    {
        private static readonly PlayerAction[] ACTIONS = 
        {
            PlayerAction.Up,
            PlayerAction.Left,
            PlayerAction.Down,
            PlayerAction.Right,
            PlayerAction.Bomb,
            PlayerAction.Special
        };

        public enum State
        {
            Normal,
            Cornered,
            Dying,
            Dead
        }

        private State m_State;

        private int m_Index;
        private int m_TriggerBombsCount;

        private bool m_Alive;

        private PlayerInput m_Input;
        private BombList m_Bombs;
        private PowerupList m_Powerups;
        private DiseaseList m_Diseases;

        private Bomb m_BombInHands;

        /* Kicked/Punched bombs */
        private List<Bomb> m_ThrownBombs;

        private Object m_Data;
        private NetConnection m_Connection;

        private int m_LastAckPacketId; // last acknowledged packet id

        private int m_WinsCount;
        private int m_SuicidesCount;

        public Player(int index)
            : base(FieldCellType.Player, 0, 0)
        {
            m_Index = index;
            m_Alive = true;

            InitPowerups();
            InitBombs();
            InitDiseases();
            InitPlayer();

            m_ThrownBombs = new List<Bomb>();
        }

        public override void Reset()
        {
            base.Reset();

            m_Input.Reset();

            SetCell(0, 0);

            m_Alive = true;

            ResetPowerups();
            ResetBombs();
            ResetDiseases();
            ResetPlayer();
            
            m_ThrownBombs.Clear();
            m_WinsCount = 0;
            m_SuicidesCount = 0;
            m_Data = null;
            m_Connection = null;
            m_LastAckPacketId = 0;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region IUpdatable

        public override void Update(float delta)
        {
            base.Update(delta);

            if (IsAlive())
            {
                UpdateInput(delta);

                if (m_BombInHands != null)
                {
                    m_BombInHands.Update(delta);
                }

                m_Diseases.Update(delta);
                TryPoops();
            }

            for (int bombIndex = 0; bombIndex < m_ThrownBombs.Count; ++bombIndex)
            {
                m_ThrownBombs[bombIndex].Update(delta);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Input

        private void UpdateInput(float delta)
        {
            m_Input.Update(delta);

            for (int i = 0; i < ACTIONS.Length; ++i)
            {
                PlayerAction action = ACTIONS[i];
                if (m_Input.IsActionJustPressed(action))
                {
                    OnActionPressed(m_Input, action);
                }
            }

            for (int i = 0; i < ACTIONS.Length; ++i)
            {
                PlayerAction action = ACTIONS[i];
                if (m_Input.IsActionJustReleased(action))
                {
                    OnActionReleased(m_Input, action);
                }
            }
        }

        public void OnActionPressed(PlayerInput playerInput, PlayerAction action)
        {
            switch (action)
            {
                case PlayerAction.Up:
                    {
                        StartMovingToDirection(Direction.UP);
                        break;
                    }

                case PlayerAction.Down:
                    {
                        StartMovingToDirection(Direction.DOWN);
                        break;
                    }

                case PlayerAction.Left:
                    {
                        StartMovingToDirection(Direction.LEFT);
                        break;
                    }

                case PlayerAction.Right:
                    {
                        StartMovingToDirection(Direction.RIGHT);
                        break;
                    }

                case PlayerAction.Bomb:
                    {
                        TryAction();
                        break;
                    }

                case PlayerAction.Special:
                    {
                        TrySpecialAction();
                        break;
                    }
            }
        }

        public void OnActionReleased(PlayerInput playerInput, PlayerAction action)
        {
            switch (action)
            {
                case PlayerAction.Left:
                    StopMovingToDirection(Direction.LEFT);
                    break;
                case PlayerAction.Right:
                    StopMovingToDirection(Direction.RIGHT);
                    break;
                case PlayerAction.Up:
                    StopMovingToDirection(Direction.UP);
                    break;
                case PlayerAction.Down:
                    StopMovingToDirection(Direction.DOWN);
                    break;
            }

            if (playerInput.GetPressedActionCount() > 0)
            {
                if (playerInput.IsActionPressed(PlayerAction.Up))
                {
                    StartMovingToDirection(Direction.UP);
                }
                else if (playerInput.IsActionPressed(PlayerAction.Down))
                {
                    StartMovingToDirection(Direction.DOWN);
                }

                if (playerInput.IsActionPressed(PlayerAction.Left))
                {
                    StartMovingToDirection(Direction.LEFT);
                }
                else if (playerInput.IsActionPressed(PlayerAction.Right))
                {
                    StartMovingToDirection(Direction.RIGHT);
                }
            }

            if (action == PlayerAction.Bomb)
            {
                TryThrowBomb();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////
        
        #region Movement

        private void StartMovingToDirection(Direction dir)
        {
            SetMoveDirection(dir);
        }

        private void StopMovingToDirection(Direction dir)
        {
            StopMoving();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Collisions

        public override bool HandleWallCollision()
        {
            return false;
        }

        public override bool HandleCollision(FieldCell cell)
        {
            if (cell.IsObstacle())
            {
                if (cell.IsBomb())
                {
                    return HandleCollision(cell.AsBomb());
                }

                return HandleObstacleCollision(cell);
            }

            if (cell.IsPowerup())
            {
                return HandleCollision(cell.AsPowerup());
            }

            if (cell.IsFlame())
            {
                return HandleCollision(cell.AsFlame());
            }

            return base.HandleCollision(cell);
        }

        private bool HandleCollision(Bomb bomb)
        {
            if (!IsMoving())
            {
                return HandleStillPlayerCollision(bomb);
            }

            if (!bomb.IsMoving())
            {
                return HandleStillBombCollision(bomb);
            }

            return HandleBombCollision(bomb);
        }

        /* Player is not moving */
        private bool HandleStillPlayerCollision(Bomb bomb)
        {
            if (bomb.IsMoving())
            {
                return bomb.HandleObstacleCollision(this);
            }

            return false;
        }

        /* Player and bomb are moving */
        private bool HandleBombCollision(Bomb bomb)
        {
            if (bomb.jellyBounced)
            {
                return MoveOutOfCollision(bomb);
            }

            if (direction == bomb.direction)
            {
                if (HasKick())
                {
                    bool canKick = false;
                    switch (direction)
                    {
                        case Direction.LEFT:
                            canKick = px - bomb.px > 0;
                            break;
                        case Direction.RIGHT:
                            canKick = px - bomb.px < 0;
                            break;
                        case Direction.UP:
                            canKick = py - bomb.py > 0;
                            break;
                        case Direction.DOWN:
                            canKick = py - bomb.py < 0;
                            break;
                    }

                    if (canKick)
                    {
                        if (TryKick(bomb))
                        {
                            return true;
                        }

                        return MoveOutOfCollision(bomb);
                    }

                    bomb.MoveOutOfCollision(this);
                    return bomb.HandleObstacleCollision(this);
                }
            }
            else // moving in different directions
            {       
                MoveOutOfCollision(this, bomb);

                if (HasKick())
                {
                    if (TryKick(bomb))
                    {                            
                        return true;
                    }
                }
            }

            return bomb.HandleObstacleCollision(this);
        }

        /* Player is moving, bomb is still */
        private bool HandleStillBombCollision(Bomb bomb)
        {
            if (HasKick())
            {
                bool canKick = false;
                switch (direction)
                {
                    case Direction.LEFT:
                        canKick = px - bomb.px > Constant.CELL_WIDTH_2;
                        break;
                    case Direction.RIGHT:
                        canKick = bomb.px - px > Constant.CELL_WIDTH_2;
                        break;
                    case Direction.UP:
                        canKick = py - bomb.py > Constant.CELL_HEIGHT_2;
                        break;
                    case Direction.DOWN:
                        canKick = bomb.py - py > Constant.CELL_HEIGHT_2;
                        break;
                }

                if (canKick && TryKick(bomb))
                {
                    return true;
                }
            }

            return MoveOutOfCollision(this, bomb);
        }

        private bool HandleCollision(PowerupCell powerup)
        {
            int powerupIndex = powerup.powerup;
            TryAddPowerup(powerupIndex);
            GetField().RemoveCell(powerup);
            return true;
        }

        private bool HandleCollision(FlameCell flame)
        {
            if (IsAlive())
            {
                Kill();
                return true;
            }

            return false;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Powerups

        private void InitPowerups()
        {
            CVar[] initials = CVars.powerupsInitials;
            CVar[] max = CVars.powerupsMax;

            int totalCount = initials.Length;
            m_Powerups = new PowerupList(totalCount);
            for (int powerupIndex = 0; powerupIndex < totalCount; ++powerupIndex)
            {
                int initialCount = initials[powerupIndex].intValue;
                int maxCount = max[powerupIndex].intValue;
                m_Powerups.Init(powerupIndex, initialCount, maxCount);
            }
        }

        private void ResetPowerups()
        {
            CVar[] initials = CVars.powerupsInitials;
            CVar[] max = CVars.powerupsMax;

            int totalCount = initials.Length;
            for (int powerupIndex = 0; powerupIndex < totalCount; ++powerupIndex)
            {
                int initialCount = initials[powerupIndex].intValue;
                int maxCount = max[powerupIndex].intValue;
                m_Powerups.Init(powerupIndex, initialCount, maxCount);
            }
        }

        public bool TryAddPowerup(int powerupIndex)
        {
            bool added = m_Powerups.Inc(powerupIndex);
            if (!added)
            {
                return false;
            }

            switch (powerupIndex)
            {
                case Powerups.Bomb:
                    {
                        m_Bombs.IncMaxActiveCount();

                        if (HasTrigger())
                        {
                            ++m_TriggerBombsCount;
                        }

                        break;
                    }

                case Powerups.Speed:
                    {
                        SetSpeed(CalcPlayerSpeed());
                        break;
                    }

                case Powerups.Flame:
                case Powerups.GoldFlame:
                    {   
                        break;
                    }

                // Trigger will drop Jelly and Boxing Glove
                case Powerups.Trigger:
                    {
                        m_TriggerBombsCount = m_Bombs.GetMaxActiveCount();

                        TryGivePowerupBack(Powerups.Jelly);
                        TryGivePowerupBack(Powerups.Punch);
                        break;
                    }

                // Jelly will drop Trigger
                // Boxing Glove will drop Trigger
                case Powerups.Jelly:
                case Powerups.Punch:
                    {
                        TryGivePowerupBack(Powerups.Trigger);
                        break;
                    }

                // Blue Hand will drop Spooge
                case Powerups.Grab:
                    {
                        TryGivePowerupBack(Powerups.Spooger);
                        break;
                    }

                // Spooge will drop Blue Hand
                case Powerups.Spooger:
                    {
                        TryGivePowerupBack(Powerups.Grab);
                        break;
                    }

                case Powerups.Disease:
                    {
                        InfectRandom(1);
                        break;
                    }

                case Powerups.Ebola:
                    {
                        InfectRandom(3);
                        break;
                    }
            }

            return true;
        }

        public void OnInfected(Diseases desease)
        {
            if (desease == Diseases.POOPS)
            {
                TryPoops();
            }
        }

        public void OnCured(Diseases desease)
        {
        }

        public void InfectRandom(int count)
        {
            m_Diseases.InfectRandom(count);
        }

        public bool HasKick()
        {
            return HasPowerup(Powerups.Kick);
        }

        public bool HasPunch()
        {
            return HasPowerup(Powerups.Punch);
        }

        public bool HasSpooger()
        {
            return HasPowerup(Powerups.Spooger);
        }

        public bool HasTrigger()
        {
            return HasPowerup(Powerups.Trigger);
        }

        public bool HasGrab()
        {
            return HasPowerup(Powerups.Grab);
        }

        private bool HasPowerup(int powerupIndex)
        {
            return m_Powerups.HasPowerup(powerupIndex);
        }

        private void TryGivePowerupBack(int powerupIndex)
        {
            if (m_Powerups.HasPowerup(powerupIndex))
            {
                switch (powerupIndex)
                {
                    case Powerups.Trigger:
                        m_TriggerBombsCount = 0;
                        break;
                }

                GivePowerupBack(powerupIndex);
            }
        }

        private void GivePowerupBack(int powerupIndex)
        {
            Debug.Assert(m_Powerups.GetCount(powerupIndex) == 1);
            m_Powerups.SetCount(powerupIndex, 0);

            GetField().PlacePowerup(powerupIndex);
        }

        private bool IsInfectedPoops()
        {
            return IsInfected(Diseases.POOPS);
        }

        private bool IsInfectedShortFuze()
        {
            return IsInfected(Diseases.SHORTFUZE);
        }

        private bool IsInfectedShortFlame()
        {
            return IsInfected(Diseases.SHORTFLAME);
        }

        private bool IsInfectedConstipation()
        {
            return IsInfected(Diseases.CONSTIPATION);
        }

        private bool IsInfected(Diseases disease)
        {
            return m_Diseases.IsInfected(disease);
        }

        private int CalcPlayerSpeed()
        {
            int speedBase = CVars.cg_playerSpeed.intValue;
            int speedAdd = CVars.cg_playerSpeedAdd.intValue;
            return speedBase + speedAdd * m_Powerups.GetCount(Powerups.Speed);
        }

        private int CalcBombsCount()
        {
            return m_Powerups.GetCount(Powerups.Bomb);
        }

        private float CalcBombTimeout()
        {
            return CVars.cg_fuzeTimeNormal.intValue * 0.001f;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Bombs

        private void InitBombs()
        {
            int maxBombs = CVars.cg_maxBomb.intValue;
            m_Bombs = new BombList(this, maxBombs);
            m_Bombs.SetMaxActiveCount(CalcBombsCount());
        }

        private void ResetBombs()
        {
            m_Bombs.Reset();
            m_Bombs.SetMaxActiveCount(CalcBombsCount());
        }

        private bool TryKick(Bomb bomb)
        {
            FieldCellSlot blockingSlot = bomb.NearSlotDir(direction);
            if (blockingSlot != null && !blockingSlot.ContainsObstacle())
            {
                KickBomb(bomb);
                return true;
            }

            return false;
        }

        private void KickBomb(Bomb bomb)
        {
            switch (direction)
            {
                case Direction.RIGHT:
                case Direction.LEFT:
                    float overlapX = OverlapX(this, bomb);
                    if (overlapX > 0)
                    {
                        MoveBackX(overlapX);
                    }
                    break;

                case Direction.UP:
                case Direction.DOWN:
                    float overlapY = OverlapY(this, bomb);
                    if (overlapY > 0)
                    {
                        MoveBackY(overlapY);
                    }
                    break;
            }

            bomb.Kick(direction);
        }

        public void OnBombBlown(Bomb bomb)
        {
        }

        private Bomb GetNextBomb()
        {
            if (IsInfectedConstipation())
            {
                return null;
            }

            return m_Bombs.GetNextBomb();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Kicked/Punched bombs

        private void AddThrownBomb(Bomb bomb)
        {
            Debug.AssertNotContains(m_ThrownBombs, bomb);
            m_ThrownBombs.Add(bomb);
        }

        private void RemoveThrownBomb(Bomb bomb)
        {
            bool removed = m_ThrownBombs.Remove(bomb);
            Debug.Assert(removed);
        }

        public void OnBombLanded(Bomb bomb)
        {
            RemoveThrownBomb(bomb);
            bomb.SetCell();
            GetField().SetBomb(bomb);
        }

        public Bomb bombInHands
        {
            get { return m_BombInHands; }
        }

        public List<Bomb> thrownBombs
        {
            get { return m_ThrownBombs; }
        }

        public bool IsHoldingBomb()
        {
            return m_BombInHands != null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Disease

        private void InitDiseases()
        {
            m_Diseases = new DiseaseList(this);
        }

        private void ResetDiseases()
        {
            m_Diseases.Reset();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Player init

        private void InitPlayer()
        {
            SetSpeed(CalcPlayerSpeed());
            m_State = State.Normal;
        }

        private void ResetPlayer()
        {
            InitPlayer();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Player input

        public void SetPlayerInput(PlayerInput input)
        {
            this.m_Input = input;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Kill

        // should be only called from PlayeList
        internal void Kill()
        {
            Debug.Assert(m_Alive);
            m_Alive = false;
            
            StopMoving();
            if (m_BombInHands != null)
            {
                m_BombInHands.active = false;
                m_BombInHands = null;
            }
        }

        internal void DeathTimerCallback(Timer timer)
        {
            GetField().RemoveCell(this);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Inheritance

        protected override void OnCellChanged(int oldCx, int oldCy)
        {
            TryPoops();
            base.OnCellChanged(oldCx, oldCy);
        }

        public override Player AsPlayer()
        {
            return this;
        }

        public override bool IsPlayer()
        {
            return true;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Actions

        public void TryAction()
        {
            bool bombSet = TryBomb();
            if (!bombSet)
            {
                if (HasSpooger())
                {
                    TrySpooger();
                }
                else if (HasGrab())
                {
                    TryGrab();
                }
            }
        }

        private void TrySpecialAction()
        {
            if (HasKick())
            {
                TryStopBomb();
            }
            if (HasTrigger())
            {
                TryTriggerBomb();
            }
            if (HasPunch())
            {
                TryPunchBomb();
            }
        }

        private void TryStopBomb()
        {
            Bomb kickedBomb = m_Bombs.GetFirstKickedBomb();
            if (kickedBomb != null)
            {
                kickedBomb.TryStopKicked();
            }
        }

        private bool TrySpooger()
        {
            Bomb underlyingBomb = GetField().GetSlot(cx, cy).GetBomb();
            if (underlyingBomb == null)
            {
                return false; // you can use spooger only when standing on the bomb
            }

            if (underlyingBomb.player != this)
            {
                return false; // you only can use spooger when standing at your own bomb
            }

            switch (direction)
            {
                case Direction.UP:
                    return TrySpooger(0, -1);

                case Direction.DOWN:
                    return TrySpooger(0, 1);

                case Direction.LEFT:
                    return TrySpooger(-1, 0);

                case Direction.RIGHT:
                    return TrySpooger(1, 0);

                default:
                    Debug.Assert(false, "Unknown direction: " + direction);
                    break;
            }

            return false;
        }

        private bool TrySpooger(int dcx, int dcy)
        {
            int uCx = cx + dcx;
            int uCy = cy + dcy;

            int numBombs = 0;
            Field field = GetField();

            while (field.ContainsNoObstacle(uCx, uCy))
            {
                Bomb nextBomb = GetNextBomb();
                if (nextBomb == null)
                {
                    break; // no bombs to apply
                }

                nextBomb.SetCell(uCx, uCy);
                field.SetBomb(nextBomb);

                uCx += dcx;
                uCy += dcy;
                ++numBombs;
            }

            return numBombs > 0;
        }

        private bool TryTriggerBomb()
        {
            Bomb triggerBomb = m_Bombs.GetFirstTriggerBomb();
            if (triggerBomb != null)
            {
                triggerBomb.Blow();
                return true;
            }

            return false;
        }

        private bool TryBomb()
        {   
            if (GetField().ContainsNoObstacle(cx, cy))
            {
                Bomb bomb = GetNextBomb();
                if (bomb != null)
                {
                    GetField().SetBomb(bomb);
                    if (HasTrigger() && m_TriggerBombsCount > 0)
                    {
                        --m_TriggerBombsCount;
                    }
                    return true;
                }
            }
            return false;
        }

        private bool TryGrab()
        {
            Bomb underlyingBomb = GetField().GetBomb(cx, cy);
            if (underlyingBomb != null)
            {
                underlyingBomb.Grab();
                m_BombInHands = underlyingBomb;
                return true;
            }
            return false;
        }

        private bool TryThrowBomb()
        {
            if (IsHoldingBomb())
            {
                AddThrownBomb(m_BombInHands);
                m_BombInHands.Throw();
                m_BombInHands = null;
                return true;
            }
            return false;
        }

        private bool TryThrowAllBombs()
        {
            int bombsCount = 0;

            Bomb nextBomb;
            while ((nextBomb = GetNextBomb()) != null)
            {
                AddThrownBomb(nextBomb);
                nextBomb.Throw();
            }

            return bombsCount > 0;
        }
        
        private bool TryPunchBomb()
        {
            FieldCellSlot slot = NearSlotDir(direction);
            Bomb bomb = slot != null ? slot.GetBomb() : null;
            if (bomb != null)
            {   
                AddThrownBomb(bomb);
                bomb.Punch();
                return true;
            }

            return false;
        }

        private bool TryPoops()
        {
            if (IsInfectedPoops())
            {
                if (HasGrab())
                {
                    return TryThrowAllBombs();
                }

                return TryBomb();
            }

            return false;
        }

        public bool TryInfect(int diseaseIndex)
        {
            return m_Diseases.TryInfect(diseaseIndex);
        }

        public bool IsInfected()
        {
            return m_Diseases.activeCount > 0;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Round

        public void IncWinsCount()
        {
            m_WinsCount++;
        }

        public void IncSuicidesCount()
        {
            m_SuicidesCount++;
        }

        public int GetWinsCount()
        {
            return m_WinsCount;
        }

        public int GetSuicidesCount()
        {
            return m_SuicidesCount;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public float GetBombTimeout()
        {
            CVar var = IsInfectedShortFuze() ? CVars.cg_fuzeTimeShort : CVars.cg_fuzeTimeNormal;
            return var.intValue * 0.001f;
        }

        public int GetBombRadius()
        {
            if (IsInfectedShortFlame())
            {
                return CVars.cg_bombShortFlame.intValue;
            }
            if (m_Powerups.HasPowerup(Powerups.GoldFlame))
            {
                return int.MaxValue;
            }
            return m_Powerups.GetCount(Powerups.Flame);
        }

        public bool IsJelly()
        {
            return HasPowerup(Powerups.Jelly);
        }

        public bool IsTrigger()
        {
            return HasTrigger() && m_TriggerBombsCount > 0;
        }

        public PowerupList powerups
        {
            get { return m_Powerups; }
        }

        public BombList bombs
        {
            get { return m_Bombs; }
        }

        public PlayerInput input
        {
            get { return m_Input; }
        }

        public NetConnection connection
        {
            get { return m_Connection; }
            set { m_Connection = value; }
        }

        public DiseaseList diseases
        {
            get { return m_Diseases; }
        }

        public int lastAckPacketId
        {
            get { return m_LastAckPacketId; }
            set { m_LastAckPacketId = value; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Getters/Setters

        public bool IsAlive()
        {
            return m_Alive;
        }

        public int GetIndex()
        {
            return m_Index;
        }

        #endregion
    }
}

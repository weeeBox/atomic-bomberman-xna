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

        private int index;
        private State state;

        private bool alive;
        public double deathTimeStamp; // TODO

        private int triggerBombsCount;

        public PlayerInput input;

        public BombList bombs;
        public PowerupList powerups;
        public DiseaseList diseases;

        private Bomb m_bombInHands;

        /* Kicked/Punched bombs */
        private List<Bomb> m_thrownBombs;

        public Object data;
        public NetConnection connection;

        public int lastAckPacketId; // last acknowledged packet id

        public Player(int index)
            : base(FieldCellType.Player, 0, 0)
        {
            this.index = index;
            alive = true;

            InitPowerups();
            InitBombs();
            InitDiseases();
            InitPlayer();

            m_thrownBombs = new List<Bomb>();
        }

        public override void Reset()
        {
            base.Reset();

            alive = true;

            ResetPowerups();
            ResetBombs();
            ResetDiseases();
            ResetPlayer();
            
            m_thrownBombs.Clear();
            data = null;
            connection = null;
            lastAckPacketId = 0;
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (IsAlive())
            {
                UpdateInput(delta);

                if (m_bombInHands != null)
                {
                    m_bombInHands.Update(delta);
                }

                diseases.Update(delta);
                TryPoops();
            }

            for (int bombIndex = 0; bombIndex < m_thrownBombs.Count; ++bombIndex)
            {
                m_thrownBombs[bombIndex].Update(delta);
            }
        }

        private void UpdateInput(float delta)
        {
            input.Update(delta);

            for (int i = 0; i < ACTIONS.Length; ++i)
            {
                PlayerAction action = ACTIONS[i];
                if (input.IsActionJustPressed(action))
                {
                    OnActionPressed(input, action);
                }
            }

            for (int i = 0; i < ACTIONS.Length; ++i)
            {
                PlayerAction action = ACTIONS[i];
                if (input.IsActionJustReleased(action))
                {
                    OnActionReleased(input, action);
                }
            }

            input.SaveState();
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

        private void StartMovingToDirection(Direction dir)
        {
            SetMoveDirection(dir);
        }

        private void StopMovingToDirection(Direction dir)
        {
            StopMoving();
        }

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

        public bool TryKill()
        {
            if (IsAlive())
            {
                Kill();
                return true;
            }

            return false;
        }

        public void Kill()
        {
            alive = false;
            deathTimeStamp = Application.CurrentTime();
            StopMoving();
            if (m_bombInHands != null)
            {
                m_bombInHands.active = false;
                m_bombInHands = null;
            }
            GetField().KillPlayer(this);
        }

        public void DeathTimerCallback(Timer timer)
        {
            GetField().RemoveCell(this);
        }

        protected override void OnCellChanged(int oldCx, int oldCy)
        {
            TryPoops();
            base.OnCellChanged(oldCx, oldCy);
        }

        public void OnBombBlown(Bomb bomb)
        {   
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

        //////////////////////////////////////////////////////////////////////////////

        #region Powerups

        private void InitPowerups()
        {
            CVar[] initials = CVars.powerupsInitials;
            CVar[] max = CVars.powerupsMax;

            int totalCount = initials.Length;
            powerups = new PowerupList(totalCount);
            for (int powerupIndex = 0; powerupIndex < totalCount; ++powerupIndex)
            {
                int initialCount = initials[powerupIndex].intValue;
                int maxCount = max[powerupIndex].intValue;
                powerups.Init(powerupIndex, initialCount, maxCount);
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
                powerups.Init(powerupIndex, initialCount, maxCount);
            }
        }

        public bool TryAddPowerup(int powerupIndex)
        {
            bool added = powerups.Inc(powerupIndex);
            if (!added)
            {
                return false;
            }

            switch (powerupIndex)
            {
                case Powerups.Bomb:
                    {
                        bombs.IncMaxActiveCount();

                        if (HasTrigger())
                        {
                            ++triggerBombsCount;
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
                        triggerBombsCount = bombs.GetMaxActiveCount();

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

        public void InfectRandom(int count)
        {
            diseases.InfectRandom(count);
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
            return powerups.HasPowerup(powerupIndex);
        }

        private void TryGivePowerupBack(int powerupIndex)
        {
            if (powerups.HasPowerup(powerupIndex))
            {
                switch (powerupIndex)
                {
                    case Powerups.Trigger:
                        triggerBombsCount = 0;
                        break;
                }

                GivePowerupBack(powerupIndex);
            }
        }

        private void GivePowerupBack(int powerupIndex)
        {
            Debug.Assert(powerups.GetCount(powerupIndex) == 1);
            powerups.SetCount(powerupIndex, 0);

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
            return diseases.IsInfected(disease);
        }

        private int CalcPlayerSpeed()
        {
            int speedBase = CVars.cg_playerSpeed.intValue;
            int speedAdd = CVars.cg_playerSpeedAdd.intValue;
            return speedBase + speedAdd * powerups.GetCount(Powerups.Speed);
        }

        private int CalcBombsCount()
        {
            return powerups.GetCount(Powerups.Bomb);
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
            bombs = new BombList(this, maxBombs);
            bombs.SetMaxActiveCount(CalcBombsCount());
        }

        private void ResetBombs()
        {
            bombs.Reset();
            bombs.SetMaxActiveCount(CalcBombsCount());
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Disease

        private void InitDiseases()
        {
            diseases = new DiseaseList(this);
        }

        private void ResetDiseases()
        {
            diseases.Reset();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Player init

        private void InitPlayer()
        {
            SetSpeed(CalcPlayerSpeed());
            state = State.Normal;
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
            this.input = input;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public override Player AsPlayer()
        {
            return this;
        }

        public override bool IsPlayer()
        {
            return true;
        }

        public bool IsAlive()
        {
            return alive;
        }

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

        private Bomb GetNextBomb()
        {
            if (IsInfectedConstipation())
            {
                return null;
            }

            return bombs.GetNextBomb();
        }

        public float GetBombTimeout()
        {
            CVar var = IsInfectedShortFuze() ? CVars.cg_fuzeTimeShort : CVars.cg_fuzeTimeNormal;
            return var.intValue * 0.001f;
        }

        public int GetIndex()
        {
            return index;
        }

        public int GetBombRadius()
        {
            if (IsInfectedShortFlame())
            {
                return CVars.cg_bombShortFlame.intValue;
            }
            if (powerups.HasPowerup(Powerups.GoldFlame))
            {
                return int.MaxValue;
            }
            return powerups.GetCount(Powerups.Flame);
        }

        public bool IsJelly()
        {
            return HasPowerup(Powerups.Jelly);
        }

        public bool IsTrigger()
        {
            return HasTrigger() && triggerBombsCount > 0;
        }

        public bool IsHoldingBomb()
        {
            return m_bombInHands != null;
        }

        public bool IsInfected()
        {
            return diseases.activeCount > 0;
        }

        public Bomb bombInHands
        {
            get { return m_bombInHands; }
        }

        public List<Bomb> thrownBombs
        {
            get { return m_thrownBombs; }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Kicked/Punched bombs

        private void AddThrownBomb(Bomb bomb)
        {
            Debug.AssertNotContains(m_thrownBombs, bomb);
            m_thrownBombs.Add(bomb);
        }

        private void RemoveThrownBomb(Bomb bomb)
        {
            bool removed = m_thrownBombs.Remove(bomb);
            Debug.Assert(removed);
        }

        public void OnBombLanded(Bomb bomb)
        {
            RemoveThrownBomb(bomb);
            bomb.SetCell();
            GetField().SetBomb(bomb);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Special actions

        private void TryStopBomb()
        {
            Bomb kickedBomb = bombs.GetFirstKickedBomb();
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
            Bomb triggerBomb = bombs.GetFirstTriggerBomb();
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
                    if (HasTrigger() && triggerBombsCount > 0)
                    {
                        --triggerBombsCount;
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
                m_bombInHands = underlyingBomb;
                return true;
            }
            return false;
        }

        private bool TryThrowBomb()
        {
            if (IsHoldingBomb())
            {
                AddThrownBomb(m_bombInHands);
                m_bombInHands.Throw();
                m_bombInHands = null;
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
            return diseases.TryInfect(diseaseIndex);
        }

        #endregion
    }
}

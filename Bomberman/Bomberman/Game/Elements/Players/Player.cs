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
using Bomberman.Content;

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

        private int m_index;
        private int m_triggerBombsCount;

        private bool m_alive;

        private PlayerInput m_input;
        private BombList m_bombs;
        private PowerupList m_powerups;
        private DiseaseList m_diseases;

        private Bomb m_bombInHands;

        /* Kicked/Punched bombs */
        private List<Bomb> m_thrownBombs;

        private NetConnection m_connection;

        private int m_lastAckPacketId; // last acknowledged packet id

        private int m_winsCount;
        private int m_suicidesCount;

        private PlayerAnimations m_animations;
        private AnimationInstance m_currentAnimation;

        private bool m_punchingBomb; // true if player is in the middle of punching a bomb (animation is playing)
        private bool m_pickingBomb; // true if player is in the middle of picking a bomb (animation is playing)

        public Player(int index)
            : base(FieldCellType.Player, 0, 0)
        {
            m_index = index;
            m_alive = true;

            InitPowerups();
            InitBombs();
            InitDiseases();
            InitPlayer();

            m_thrownBombs = new List<Bomb>();
        }

        public override void Reset()
        {
            base.Reset();

            m_input.Reset();

            SetCell(0, 0);

            m_alive = true;

            ResetPowerups();
            ResetBombs();
            ResetDiseases();
            ResetPlayer();
            ResetAnimation();
            
            m_thrownBombs.Clear();
            m_winsCount = 0;
            m_suicidesCount = 0;
            m_connection = null;
            m_lastAckPacketId = 0;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region IUpdatable

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

                m_diseases.Update(delta);
            }

            if (m_currentAnimation != null)
            {
                m_currentAnimation.Update(delta);
            }

            for (int bombIndex = 0; bombIndex < m_thrownBombs.Count; ++bombIndex)
            {
                m_thrownBombs[bombIndex].Update(delta);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Input

        private void UpdateInput(float delta)
        {
            if (m_input.IsActive)
            {
                m_input.Update(delta);
            }

            for (int i = 0; i < ACTIONS.Length; ++i)
            {
                PlayerAction action = ACTIONS[i];
                if (m_input.IsActionJustPressed(action))
                {
                    OnActionPressed(m_input, action);
                }
            }

            for (int i = 0; i < ACTIONS.Length; ++i)
            {
                PlayerAction action = ACTIONS[i];
                if (m_input.IsActionJustReleased(action))
                {
                    OnActionReleased(m_input, action);
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
            UpdateAnimation();
        }

        private void StopMovingToDirection(Direction dir)
        {
            StopMoving();
            UpdateAnimation();
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
            if (bomb.isJellyBounced)
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
                GetField().KillPlayer(this);
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
            m_powerups = new PowerupList(this, totalCount);
            for (int powerupIndex = 0; powerupIndex < totalCount; ++powerupIndex)
            {
                int initialCount = initials[powerupIndex].intValue;
                int maxCount = max[powerupIndex].intValue;
                m_powerups.Init(powerupIndex, initialCount, maxCount);
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
                m_powerups.Init(powerupIndex, initialCount, maxCount);
            }
        }

        public bool TryAddPowerup(int powerupIndex)
        {
            bool added = m_powerups.Inc(powerupIndex);
            if (!added)
            {
                return false;
            }

            switch (powerupIndex)
            {
                case Powerups.Bomb:
                    {
                        m_bombs.IncMaxActiveCount();

                        if (HasTrigger())
                        {
                            ++m_triggerBombsCount;
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
                        m_triggerBombsCount = m_bombs.GetMaxActiveCount();

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
            m_diseases.InfectRandom(count);
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
            return m_powerups.HasPowerup(powerupIndex);
        }

        private void TryGivePowerupBack(int powerupIndex)
        {
            if (m_powerups.HasPowerup(powerupIndex))
            {
                switch (powerupIndex)
                {
                    case Powerups.Trigger:
                        m_triggerBombsCount = 0;
                        break;
                }

                GivePowerupBack(powerupIndex);
            }
        }

        private void GivePowerupBack(int powerupIndex)
        {
            Debug.Assert(m_powerups.GetCount(powerupIndex) == 1);
            m_powerups.SetCount(powerupIndex, 0);

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
            return m_diseases.IsInfected(disease);
        }

        private int CalcPlayerSpeed()
        {
            int speedBase = CVars.cg_playerSpeed.intValue;
            int speedAdd = CVars.cg_playerSpeedAdd.intValue;
            return speedBase + speedAdd * m_powerups.GetCount(Powerups.Speed);
        }

        private int CalcBombsCount()
        {
            return m_powerups.GetCount(Powerups.Bomb);
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
            m_bombs = new BombList(this, maxBombs);
            m_bombs.SetMaxActiveCount(CalcBombsCount());
        }

        private void ResetBombs()
        {
            m_bombs.Reset();
            m_bombs.SetMaxActiveCount(CalcBombsCount());
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

            return m_bombs.GetNextBomb();
        }

        #endregion

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

        public Bomb bombInHands
        {
            get { return m_bombInHands; }
        }

        public List<Bomb> thrownBombs
        {
            get { return m_thrownBombs; }
        }

        public bool IsHoldingBomb()
        {
            return m_bombInHands != null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Disease

        private void InitDiseases()
        {
            m_diseases = new DiseaseList(this);
        }

        private void ResetDiseases()
        {
            m_diseases.Reset();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Player init

        private void InitPlayer()
        {
            SetSpeed(CalcPlayerSpeed());
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
            this.m_input = input;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Kill

        // should be only called from PlayeList
        internal void Kill()
        {
            Debug.Assert(m_alive);
            m_alive = false;
            
            StopMoving();
            if (m_bombInHands != null)
            {
                m_bombInHands.isActive = false;
                m_bombInHands = null;
            }

            m_diseases.CureAll();

            UpdateAnimation();
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
            Bomb kickedBomb = m_bombs.GetFirstKickedBomb();
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
            Bomb triggerBomb = m_bombs.GetFirstTriggerBomb();
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
                    if (HasTrigger() && m_triggerBombsCount > 0)
                    {
                        --m_triggerBombsCount;
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

                IsPickingUpBomb = true;
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

                IsPickingUpBomb = false;
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
            IsPunchingBomb = true;

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
            return m_diseases.TryInfect(diseaseIndex);
        }

        public bool IsInfected()
        {
            return m_diseases.activeCount > 0;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Round

        public void IncWinsCount()
        {
            m_winsCount++;
        }

        public void IncSuicidesCount()
        {
            m_suicidesCount++;
        }

        public int GetWinsCount()
        {
            return m_winsCount;
        }

        public int GetSuicidesCount()
        {
            return m_suicidesCount;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Animations

        private void InitAnimation()
        {
            m_currentAnimation = new AnimationInstance();
            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            PlayerAnimations.Id id;
            PlayerAnimations.Id currentId = (PlayerAnimations.Id)m_currentAnimation.id;
            AnimationInstance.Mode mode = AnimationInstance.Mode.Looped;

            if (IsAlive())
            {
                if (IsPunchingBomb)
                {
                    if (currentId == PlayerAnimations.Id.PunchBomb)
                    {
                        return; // don't play animation again
                    }

                    id = PlayerAnimations.Id.PunchBomb;
                    mode = AnimationInstance.Mode.Normal;
                }
                else if (IsPickingUpBomb)
                {
                    id = PlayerAnimations.Id.PickupBomb;
                    mode = AnimationInstance.Mode.Normal;
                }
                else if (IsHoldingBomb())
                {
                    id = IsMoving() ? PlayerAnimations.Id.WalkBomb : PlayerAnimations.Id.StandBomb;
                    mode = AnimationInstance.Mode.Normal;
                }
                else if (IsMoving())
                {
                    id = PlayerAnimations.Id.Walk;
                }
                else
                {
                    id = PlayerAnimations.Id.Stand;
                }
            }
            else
            {
                id = PlayerAnimations.Id.Die;
                mode = AnimationInstance.Mode.Normal;
            }

            Animation animation = m_animations.Find(id, direction);
            m_currentAnimation.Init(animation, mode);
            m_currentAnimation.id = (int)id;
            m_currentAnimation.animationDelegate = AnimationFinishedCallback;
        }

        private void ScheduleAnimationUpdate()
        {
            ScheduleTimerOnce(UpdateAnimationCallback);
        }

        private void UpdateAnimationCallback(Timer timer)
        {
            UpdateAnimation();
        }

        private void ResetAnimation()
        {
            m_pickingBomb = false;
            m_punchingBomb = false;

            UpdateAnimation();
        }

        private void AnimationFinishedCallback(AnimationInstance animation)
        {
            PlayerAnimations.Id id = (PlayerAnimations.Id)animation.id;
            switch (id)
            {
                case PlayerAnimations.Id.Die:
                {
                    Debug.Assert(!IsAlive());
                    RemoveFromField();
                    break;
                }

                case PlayerAnimations.Id.PunchBomb:
                {
                    IsPunchingBomb = false;
                    break;
                }

                case PlayerAnimations.Id.PickupBomb:
                {
                    IsPickingUpBomb = false;
                    break;
                }
            }
        }

        private bool IsPickingUpBomb
        {
            get { return m_pickingBomb; }
            set
            {   
                m_pickingBomb = value;
                ScheduleAnimationUpdate();
            }
        }

        private bool IsPunchingBomb
        {
            get { return m_punchingBomb; }
            set
            {   
                m_punchingBomb = value;
                ScheduleAnimationUpdate();
            }
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
            if (m_powerups.HasPowerup(Powerups.GoldFlame))
            {
                return int.MaxValue;
            }
            return m_powerups.GetCount(Powerups.Flame);
        }

        public bool IsJelly()
        {
            return HasPowerup(Powerups.Jelly);
        }

        public bool IsTrigger()
        {
            return HasTrigger() && m_triggerBombsCount > 0;
        }

        public PowerupList powerups
        {
            get { return m_powerups; }
        }

        public BombList bombs
        {
            get { return m_bombs; }
        }

        public PlayerInput input
        {
            get { return m_input; }
        }

        public NetConnection connection
        {
            get { return m_connection; }
            set { m_connection = value; }
        }

        public DiseaseList diseases
        {
            get { return m_diseases; }
        }

        public int lastAckPacketId
        {
            get { return m_lastAckPacketId; }
            set { m_lastAckPacketId = value; }
        }

        public PlayerAnimations animations
        {
            get { return m_animations; }
            set 
            { 
                m_animations = value;
                if (m_animations != null)
                {
                    InitAnimation();
                }
            }
        }

        public BombAnimations bombAnimations
        {
            set
            {
                for (int i = 0; i < bombs.array.Length; ++i)
                {
                    bombs.array[i].animations = value;
                }
            }
        }

        public AnimationInstance currentAnimation
        {
            get { return m_currentAnimation; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Getters/Setters

        public bool IsAlive()
        {
            return m_alive;
        }

        public int GetIndex()
        {
            return m_index;
        }

        #endregion
    }
}

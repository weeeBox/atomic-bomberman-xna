using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Items;
using Bomberman.Game.Elements.Players.Input;
using Bomberman.Game.Elements.Fields;
using BomberEngine.Debugging;

namespace Bomberman.Game.Elements.Players
{
    public class Player : MovableCell, PlayerInputListener
    {   
        private int index;

        private bool alive;

        private int bombRadius;
        private float bombTimeout;
        private int triggerBombsCount;

        private PlayerInput input;

        private BombList bombs;
        public PowerupList powerups;

        private Bomb m_bombInHands;

        /* Kicked/Punched bombs */
        private List<Bomb> m_thrownBombs;
        
        public Player(int index, PlayerInput input)
            : base(0, 0)
        {
            this.index = index;
            this.input = input;
            input.SetListener(this);

            alive = true;

            InitPowerups();
            InitBombs();
            InitPlayer();

            m_thrownBombs = new List<Bomb>();
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (m_bombInHands != null)
            {
                m_bombInHands.Update(delta);
            }

            for (int bombIndex = 0; bombIndex < m_thrownBombs.Count; ++bombIndex)
            {
                m_thrownBombs[bombIndex].Update(delta);
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
                    TryAction();
                    break;
                }

                case PlayerAction.Special:
                {
                    TrySpecialAction();
                    break;
                }

                case PlayerAction.CheatAddBomb:
                {
                    TryAddPowerup(Powerups.Bomb);
                    break;
                }
            }
        }

        public void OnActionReleased(PlayerInput playerInput, PlayerAction action)
        {
            StopMoving();

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

            if (action == PlayerAction.Bomb)
            {
                TryThrowBomb();
            }
        }

        public void OnActonsReleased(PlayerInput playerInput)
        {
            StopMoving();
        }

        public override void HitWall()
        {
        }

        public override void HitObstacle(FieldCell obstacle)
        {   
        }

        protected override void OnCellChanged(int oldCx, int oldCy)
        {   
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Powerups

        private static readonly int[] POWERUPS_INITIALS =
        {
            Settings.VAL_PU_INIT_BOMB,
            Settings.VAL_PU_INIT_FLAME,
            Settings.VAL_PU_INIT_DISEASE,
            Settings.VAL_PU_INIT_ABILITY_KICK,
            Settings.VAL_PU_INIT_EXTRA_SPEED,
            Settings.VAL_PU_INIT_ABLITY_PUNCH,
            Settings.VAL_PU_INIT_ABILITY_GRAB,
            Settings.VAL_PU_INIT_SPOOGER,
            Settings.VAL_PU_INIT_GOLDFLAME,
            Settings.VAL_PU_INIT_TRIGGER,
            Settings.VAL_PU_INIT_JELLY,
            Settings.VAL_PU_INIT_EBOLA,
            Settings.VAL_PU_INIT_RANDOM,
        };

        private static readonly int[] POWERUPS_MAX =
        {
            Settings.VAL_PU_MAX_BOMB,
            Settings.VAL_PU_MAX_FLAME,
            Settings.VAL_PU_MAX_DISEASE,
            Settings.VAL_PU_MAX_ABILITY_KICK,
            Settings.VAL_PU_MAX_EXTRA_SPEED,
            Settings.VAL_PU_MAX_ABILITY_PUNCH,
            Settings.VAL_PU_MAX_ABILITY_GRAB,
            Settings.VAL_PU_MAX_SPOOGER,
            Settings.VAL_PU_MAX_GOLDFLAME,
            Settings.VAL_PU_MAX_TRIGGER,
            Settings.VAL_PU_MAX_JELLY,
            Settings.VAL_PU_MAX_EBOLA,
            Settings.VAL_PU_MAX_RANDOM,
        };

        private void InitPowerups()
        {
            int totalCount = POWERUPS_INITIALS.Length;
            powerups = new PowerupList(totalCount);
            for (int powerupIndex = 0; powerupIndex < totalCount; ++powerupIndex)
            {
                int initialCount = Settings.Get(POWERUPS_INITIALS[powerupIndex]);
                int maxCount = Settings.Get(POWERUPS_MAX[powerupIndex]);
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
                    bombRadius = CalcBombRadius();
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
            }

            return true;
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

        private int CalcPlayerSpeed()
        {
            int speedBase = Settings.Get(Settings.VAL_PLAYER_SPEED);
            int speedAdd = Settings.Get(Settings.VAL_PLAYER_SPEED_ADD);
            return speedBase  + speedAdd * powerups.GetCount(Powerups.Speed);
        }

        private int CalcBombsCount()
        {
            return powerups.GetCount(Powerups.Bomb);
        }

        private int CalcBombRadius()
        {
            if (powerups.HasPowerup(Powerups.GoldFlame))
            {
                return int.MaxValue;
            }
            return powerups.GetCount(Powerups.Flame);
        }

        private float CalcBombTimeout()
        {
            return Settings.Get(Settings.VAL_FUZE_TIME_NORMAL) * 0.001f;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Bombs

        private void InitBombs()
        {
            int maxBombs = Settings.Get(Settings.VAL_PU_MAX_BOMB);
            bombs = new BombList(this, maxBombs);
            bombs.SetMaxActiveCount(CalcBombsCount());

            bombTimeout = CalcBombTimeout();
            bombRadius = CalcBombRadius();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Player init

        private void InitPlayer()
        {
            SetSpeed(CalcPlayerSpeed());
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
            Field field = GetField();
            FieldCell underlyingCell = field.GetCell(cx, cy);
            if (underlyingCell.IsEmpty())
            {
                Bomb bomb = bombs.GetNextBomb();
                if (bomb != null)
                {
                    field.SetBomb(bomb);
                    if (HasTrigger() && triggerBombsCount > 0)
                    {
                        --triggerBombsCount;
                    }
                }
            }
            else if (HasSpooger())
            {
                TrySpooger();
            }
            else if (HasGrab())
            {
                TryGrab();
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

        public float GetBombTimeout()
        {
            return bombTimeout;
        }

        public int GetIndex()
        {
            return index;
        }

        public int GetBombRadius()
        {
            return bombRadius;
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

        public void BombLanded(Bomb bomb)
        {
            RemoveThrownBomb(bomb);
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
                kickedBomb.SetCell();
                kickedBomb.StopMoving();
            }
        }

        private bool TrySpooger()
        {
            FieldCell underlyingCell = GetField().GetCell(cx, cy);
            if (!underlyingCell.IsBomb())
            {
                return false; // you can use spooger only when standing on the bomb
            }

            Bomb underlyingBomb = underlyingCell.AsBomb();
            if (underlyingBomb.GetPlayer() != this)
            {
                return false; // you only can use spooger when standing at your own bomb
            }

            Direction direction = GetDirection();
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

            FieldCell underlyingCell;
            while ((underlyingCell = field.GetCell(uCx, uCy)) != null && !underlyingCell.IsObstacle())
            {
                Bomb nextBomb = bombs.GetNextBomb();
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

        private bool TryGrab()
        {
            Bomb underlyingBomb = GetField().GetCell(cx, cy).AsBomb();
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

        private bool TryPunchBomb()
        {
            FieldCell cell = NearCellDir(direction);
            if (cell != null && cell.IsBomb())
            {
                Bomb bomb = cell.AsBomb();
                AddThrownBomb(bomb);
                bomb.Punch();
                return true;
            }

            return false;
        }

        #endregion
    }
}

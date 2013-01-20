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

        private PlayerInput input;

        private BombList bombs;
        public PowerupList powerups;
        
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
                    TrySetBomb();
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
                    break;
                }

                case Powerups.Speed:
                {
                    speed = CalcPlayerSpeed();
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

        public bool CanKick()
        {
            return HasPowerup(Powerups.Kick);
        }

        private bool HasPowerup(int powerupIndex)
        {
            return powerups.HasPowerup(powerupIndex);
        }

        private void TryGivePowerupBack(int powerupIndex)
        {
            if (powerups.HasPowerup(powerupIndex))
            {
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
            speed = CalcPlayerSpeed();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public override bool IsPlayer()
        {
            return true;
        }

        public bool IsAlive()
        {
            return alive;
        }
        
        public void TrySetBomb()
        {
            Field field = GetField();
            if (field.GetCell(cx, cy).IsEmpty())
            {
                Bomb bomb = bombs.GetBomb();
                if (bomb != null)
                {
                    field.SetBomb(bomb);
                }
            }
        }

        private void TrySpecialAction()
        {
            if (CanKick())
            {
                Bomb kickedBomb = bombs.GetFirstKickedBomb();
                if (kickedBomb != null)
                {
                    kickedBomb.SetCell();
                    kickedBomb.StopMoving();
                }
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
            return HasPowerup(Powerups.Trigger);
        }
    }
}

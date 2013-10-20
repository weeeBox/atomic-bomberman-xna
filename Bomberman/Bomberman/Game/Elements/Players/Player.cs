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

        private bool m_ready;

        public bool needsFieldState;
        public bool needsRoundResults;
        
        private PlayerInput m_input;
        private BombList m_bombs;
        private PowerupList m_powerups;
        private DiseaseList m_diseases;

        private Bomb m_bombInHands;

        /* Kicked/Punched bombs */
        private List<Bomb> m_thrownBombs;

        private NetConnection m_connection;

        private int m_lastAckPacketId;  // last acknowledged packet id
        private int m_lastSentPacketId; // last sent packet id

        private float m_errDx;
        private float m_errDy;

        private int m_winsCount;
        private int m_suicidesCount;

        private PlayerAnimations m_animations;
        private AnimationInstance m_currentAnimation;

        private bool m_punchingBomb; // true if player is in the middle of punching a bomb (animation is playing)
        private bool m_pickingBomb; // true if player is in the middle of picking a bomb (animation is playing)

        private static Player[] s_tempArray;

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

            ResetNetworkState();
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

            ResetNetworkState();
        }

        public void ResetNetworkState()
        {
            m_ready = false;
            needsFieldState = true;
            needsRoundResults = true;
        }

        public override void Destroy()
        {
            base.Destroy();
            m_connection = null;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region IUpdatable

        public override void Update(float delta)
        {
            base.Update(delta);

            if (IsAlive)
            {
                UpdateInput(delta);

                if (m_bombInHands != null)
                {
                    m_bombInHands.Update(delta);
                }

                m_diseases.Update(delta);
            }

            UpdateAnimation(delta);

            for (int bombIndex = 0; bombIndex < m_thrownBombs.Count; ++bombIndex)
            {
                m_thrownBombs[bombIndex].Update(delta);
            }
        }

        public override void UpdateDumb(float delta)
        {
            base.UpdateDumb(delta);
            if (IsAlive)
            {
                UpdateInput(delta);
            }
        }


        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Movable

        public override void UpdateMoving(float delta)
        {
            float offset = GetSpeed() * delta;

            float dx = 0;
            float dy = 0;

            float oldPx = px;
            float oldPy = py;

            switch (direction)
            {
                case Direction.LEFT:
                case Direction.RIGHT:
                {
                    dx = moveKx * offset;
                    break;
                }

                case Direction.UP:
                case Direction.DOWN:
                {
                    dy = moveKy * offset;
                    break;
                }
            }

            Move(dx, dy);

            // we need to calculate static collisions right away to check if player's
            // movement is blocked by a static or a still object
            GetField().CheckStaticCollisions(this);

            if (IsAlive)
            {
                switch (direction)
                {
                    case Direction.LEFT:
                    case Direction.RIGHT:
                    {
                        bool blocked = Math.Abs(px - oldPx) < 0.01f;
                        dx = 0.0f;
                        dy = GetMoveTargetDy(delta, blocked);
                        break;
                    }

                    case Direction.UP:
                    case Direction.DOWN:
                    {
                        bool blocked = Math.Abs(px - oldPx) < 0.01f;
                        dx = GetMoveTargetDx(delta, blocked);
                        dy = 0.0f;
                        break;
                    }
                }

                Move(dx, dy);
            }
        }

        /* Player needs to overcome obstacles */
        private float GetMoveTargetDx(float delta, bool blocked)
        {
            float xOffset;

            if (blocked)
            {
                float cOffset = CenterOffX;
                if (Math.Abs(cOffset) < 0.01f) // if target offset is really small (more like calculation error) - don't try to come around obstacle
                {
                    return -cOffset;
                }

                int dcx = Math.Sign(cOffset);
                int dcy = Math.Sign(moveKy);

                Debug.Assert(dcx != 0);
                Debug.Assert(dcy != 0);

                if (HasNearObstacle(0, dcy)) // can't go ahead?
                {
                    if (!HasNearObstacle(dcx, dcy)) // it's ok to take the shorter path
                    {
                        xOffset = Util.Cx2Px(cx + dcx) - px;
                    }
                    else if (!HasNearObstacle(-dcx, dcy)) // it's ok to take the longer path
                    {
                        xOffset = Util.Cx2Px(cx - dcx) - px;
                    }
                    else // no way to go
                    {
                        return 0.0f;
                    }
                }
                else
                {
                    xOffset = Util.TargetPxOffset(px);
                }
            }
            else
            {
                xOffset = Util.TargetPxOffset(px);
            }
            return xOffset < 0 ? Math.Max(xOffset, -delta * GetSpeed()) : Math.Min(xOffset, delta * GetSpeed());
        }

        /* Player needs to overcome obstacles */
        private float GetMoveTargetDy(float delta, bool blocked)
        {
            float yOffset;

            if (blocked)
            {
                float cOffset = CenterOffY;
                if (Math.Abs(cOffset) < 0.01f) // if target offset is really small (more like calculation error) - don't try to come around obstacle
                {
                    return -cOffset;
                }

                int dcx = Math.Sign(moveKx);
                int dcy = Math.Sign(cOffset);

                Debug.Assert(dcx != 0);
                Debug.Assert(dcy != 0);

                if (HasNearObstacle(dcx, 0)) // can't go ahead?
                {
                    if (!HasNearObstacle(dcx, dcy)) // it's ok to take the shorter path
                    {
                        yOffset = Util.Cy2Py(cy + dcy) - py;
                    }
                    else if (!HasNearObstacle(dcx, -dcy)) // it's ok to take the longer path
                    {
                        yOffset = Util.Cy2Py(cy - dcy) - py;
                    }
                    else // no way to go
                    {
                        return 0.0f;
                    }
                }
                else
                {
                    yOffset = Util.TargetPyOffset(py);
                }
            }
            else
            {
                yOffset = Util.TargetPyOffset(py);
            }

            return yOffset < 0 ? Math.Max(yOffset, -delta * GetSpeed()) : Math.Min(yOffset, delta * GetSpeed());
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

            if (!GetField().IsGameDumbMuliplayerClient || IsNetworkPlayer)
            {
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
                case PlayerAction.Right:
                case PlayerAction.Up:
                case PlayerAction.Down:
                    StopMoving();
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

        public void StartMovingToDirection(Direction dir)
        {
            if (IsInfected(Diseases.REVERSED))
            {
                dir = Util.Opposite(dir);
            }

            SetMoveDirection(dir);
            UpdateAnimation();
        }

        public override void StopMoving()
        {
            base.StopMoving();
            UpdateAnimation();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Collisions

        public override bool HandleCollision(FieldCell cell)
        {
            if (IsAlive)
            {
                return base.HandleCollision(cell);
            }

            return false;
        }

        protected override bool HandleCollision(MovableCell other)
        {
            if (other.IsBomb())
            {
                return HandleCollision(other.AsBomb());
            }

            return false;
        }

        internal bool HandleCollision(Bomb bomb)
        {
            if (IsMoving())
            {
                if (bomb.IsMoving())
                {
                    return HandlePlayerMovingBombMovingCollision(bomb);
                }
                return HandlePlayerMovingBombStillCollision(bomb);
            }

            if (bomb.IsMoving())
            {
                return HandlePlayerStillBombMovingCollision(bomb);
            }

            return HandlePlayerStillBombStillCollision(bomb);
        }

        /* Player is moving, bomb is moving */
        private bool HandlePlayerMovingBombMovingCollision(Bomb bomb)
        {
            Debug.Assert(IsMoving());
            Debug.Assert(bomb.IsMoving());

            bool hasKick = HasKick();

            if (direction == Util.Opposite(bomb.direction)) // moving in opposite directions
            {
                if (IsMovingTowards(bomb))
                {
                    if (CheckCell2BoundsCollision(bomb) || CheckBounds2CellCollision(bomb))
                    {
                        if (hasKick)
                        {
                            // move bomb out of player`s cell
                            bomb.MoveOutOfCell(this);

                            // kick in the moving direction
                            TryKick(bomb);

                            return true;
                        }

                        // treat player as an obstacle
                        bomb.HandleObstacleCollistion(this);

                        // make sure player is out of bomb`s cell
                        MoveOutOfCell(bomb);

                        return true;
                    }
                }
            }
            else if (direction == bomb.direction) // moving in the same direction
            {   
                if (bomb.IsMovingTowards(this)) // bomb`s following player?
                {
                    if (bomb.CheckBounds2CellCollision(this)) // bomb bounds should collide player`s cell
                    {
                        // treat player as an obstacle
                        bomb.HandleObstacleCollistion(this);

                        return true;
                    }
                }
                else if (IsMovingTowards(bomb)) // player`s following bomb
                {
                    if (CheckCell2BoundsCollision(bomb) || CheckBounds2CellCollision(bomb))
                    {
                        if (hasKick)
                        {
                            // kick in the moving direction
                            TryKick(bomb);

                            return true;
                        }

                        // make sure player is out of bomb`s cell
                        MoveOutOfCell(bomb);

                        return true;
                    }
                }
            }
            else // player and bomb move in perpendicular directions
            {
                if (CheckCell2CellCollision(bomb))
                {
                    return false;
                }

                if (CheckBounds2CellCollision(bomb))
                {   
                    if (IsMovingTowards(bomb))
                    {
                        if (hasKick)
                        {
                            TryKick(bomb);
                        }

                        MoveOutOfCell(bomb);

                        return true;
                    }
                }

                if (bomb.CheckBounds2BoundsCollision(this))
                {
                    if (bomb.IsMovingTowards(this))
                    {
                        bomb.HandleObstacleCollistion(this);
                        return true;
                    }
                }
            }

            return false;
        }

        /* Player is moving, bomb is still */
        private bool HandlePlayerMovingBombStillCollision(Bomb bomb)
        {
            Debug.Assert(IsMoving());
            Debug.Assert(!bomb.IsMoving());

            if (CheckCell2CellCollision(bomb)) // player and bomb share the cell
            {
                return false;
            }

            if (CheckBounds2CellCollision(bomb)) // player bounds collide bomb`s cell
            {
                if (IsMovingTowards(bomb)) // check if player is moving towards the bomb
                {
                    if (HasKick())
                    {
                        TryKick(bomb);
                    }
                    else
                    {
                        MoveOutOfCell(bomb);
                    }

                    return true;
                }
            }

            return false;
        }

        /* Player is still, bomb is moving */
        private bool HandlePlayerStillBombMovingCollision(Bomb bomb)
        {
            Debug.Assert(!IsMoving());
            Debug.Assert(bomb.IsMoving());

            if (CheckCell2BoundsCollision(bomb)) // player`s cell should collide bomb`s bounds
            {
                if (bomb.IsMovingTowards(this)) // bomb is moving towards the player
                {
                    bomb.HandleObstacleCollistion(this);
                    return true;
                }
            }
            
            return false;
        }

        /* Player is still, bomb is still */
        private bool HandlePlayerStillBombStillCollision(Bomb bomb)
        {
            Debug.Assert(!IsMoving());
            Debug.Assert(!bomb.IsMoving());

            return false;
        }

        private bool HandleCollision(FlameCell cell)
        {
            GetField().KillPlayer(this);
            return true;
        }

        private bool HandleCollision(PowerupCell cell)
        {
            int powerup = cell.powerup;
            TryAddPowerup(powerup);

            cell.RemoveFromField();
            return true;
        }

        protected override bool HandleStaticCollision(FieldCell other)
        {
            if (other.IsFlame())
            {
                return HandleCollision(other.AsFlame());
            }

            if (other.IsPowerup())
            {   
                return HandleCollision(other.AsPowerup());
            }

            return base.HandleStaticCollision(other);
        }

        public override bool HandleWallCollision()
        {
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
                    UpdatePlayerSpeed();
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

        public virtual void OnInfected(Diseases disease)
        {
            switch (disease)
            {
                case Diseases.POOPS:
                    TryPoops();
                    break;

                case Diseases.CRACK:
                case Diseases.MOLASSES:
                    UpdatePlayerSpeed();
                    break;

                case Diseases.REVERSED:
                    StartReversed();
                    break;

                case Diseases.SWAP:
                    Swap();
                    break;

                case Diseases.HYPERSWAP:
                    SwapAll();
                    break;
            }
        }

        public virtual void OnCured(Diseases disease)
        {
            switch (disease)
            {
                case Diseases.CRACK:
                case Diseases.MOLASSES:
                    UpdatePlayerSpeed();
                    break;

                case Diseases.REVERSED:
                    StopReversed();
                    break;
            }
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

        private bool IsInfected(Diseases disease)
        {
            return m_diseases.IsInfected(disease);
        }

        private void UpdatePlayerSpeed()
        {
            int newSpeed = CalcPlayerSpeed();
            SetSpeed(newSpeed);
        }

        private int CalcPlayerSpeed()
        {
            if (IsInfected(Diseases.MOLASSES))
            {
                return CVars.cg_playerSpeedMolasses.intValue;
            }
            if (IsInfected(Diseases.CRACK))
            {
                return CVars.cg_playerSpeedCrack.intValue;
            }

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

        private void StartReversed()
        {
            if (IsMoving())
            {
                StartMovingToDirection(direction);
            }
        }

        private void StopReversed()
        {
            if (IsMoving())
            {
                StartMovingToDirection(Util.Opposite(direction));
            }
        }

        private void Swap()
        {
            PlayerList players = GetField().GetPlayers();

            Player[] temp = GetTempArray(players.GetCount());
            int count = players.GetAlivePlayers(temp, this);

            if (count > 0)
            {
                int index = MathHelp.NextInt(count);
                Swap(temp[index]);
                ArrayUtils.Clear(temp);
            }
        }

        private void SwapAll()
        {
            PlayerList players = GetField().GetPlayers();

            Player[] temp = GetTempArray(players.GetCount());
            int count = players.GetAlivePlayers(temp);

            if (count > 1)
            {
                ArrayUtils.Shuffle(temp, count);
                for (int i = 1; i < count; i += 2)
                {
                    temp[i - 1].Swap(temp[i]);
                }
                ArrayUtils.Clear(temp);
            }
        }

        private void Swap(Player other)
        {
            float tpx = other.px;
            float tpy = other.py;

            other.SetPos(px, py);
            SetPos(tpx, tpy);
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
            Debug.Assert(HasKick());
            Debug.Assert(IsMoving());

            MoveOutOfCell(bomb);

            if (!bomb.HasNearObstacle(direction))
            {
                KickBomb(bomb);
                return true;
            }
            
            return false;
        }

        private void KickBomb(Bomb bomb)
        {
            Debug.Assert(IsMoving());
            Debug.Assert(HasKick());

            // kick in the moving direction
            bomb.Kick(direction);

            // make sure player is out of the bomb`s cell
            MoveOutOfCell(bomb);
        }

        public void OnBombBlown(Bomb bomb)
        {
            TrySchedulePoops();
        }

        public Bomb GetNextBomb()
        {
            if (IsInfected(Diseases.CONSTIPATION))
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

        public bool TryAction()
        {
            bool bombSet = TryBomb();
            if (!bombSet)
            {
                if (HasSpooger())
                {
                    return TrySpooger();
                }
                if (HasGrab())
                {
                    return TryGrab();
                }

                return false;
            }

            return true;
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

            FieldCellSlot slot = GetNearSlot(direction);
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
            if (IsInfected(Diseases.POOPS))
            {
                if (HasGrab())
                {
                    return TryThrowAllBombs();
                }

                return TryBomb();
            }

            return false;
        }

        private bool TrySchedulePoops()
        {
            if (IsInfected(Diseases.POOPS))
            {
                ScheduleTimerOnce(TrySchedulePoopsCallback);
                return true;
            }

            return false;
        }

        private void TrySchedulePoopsCallback(Timer timer)
        {
            TryPoops();
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

        public int winsCount
        {
            get { return m_winsCount; }
            set { m_winsCount = value; }
        }

        public int suicidesCount
        {
            get { return m_suicidesCount; }
            set { m_suicidesCount = value; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Animations

        private void InitAnimation()
        {
            m_currentAnimation = new AnimationInstance();
            UpdateAnimation();
        }

        public override void UpdateAnimation(float delta)
        {
            if (m_currentAnimation != null)
            {
                m_currentAnimation.Update(delta);
            }
        }

        private void UpdateAnimation()
        {
            PlayerAnimations.Id id;
            PlayerAnimations.Id currentId = (PlayerAnimations.Id)m_currentAnimation.id;
            AnimationInstance.Mode mode = AnimationInstance.Mode.Looped;

            if (IsAlive)
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
                    Animation newAnimation = m_animations.Find(id, direction);
                    if (m_currentAnimation.Animation == newAnimation)
                    {
                        return;
                    }
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
                    Debug.Assert(!IsAlive);
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

        #region Network

        /* Sets player state received from the server as a part of game packet */
        internal void UpdateFromNetwork(float newPx, float newPy, bool moving, Direction newDir, float newSpeed)
        {
            m_errDx = px - newPx;
            m_errDy = py - newPy;

            if (px != newPx || py != newPy)
            {   
                SetPos(newPx, newPy);
            }

            if (!moving)
            {
                if (IsMoving())
                {
                    StopMoving();
                }
            }
            else
            {
                SetSpeed(newSpeed);
                if (newDir != direction)
                {
                    StartMovingToDirection(newDir);
                }
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public float GetBombTimeout()
        {
            CVar var = IsInfected(Diseases.SHORTFUZE) ? CVars.cg_fuzeTimeShort : CVars.cg_fuzeTimeNormal;
            return var.intValue * 0.001f;
        }

        public int GetBombRadius()
        {
            if (IsInfected(Diseases.SHORTFLAME))
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
            protected set { m_diseases = value; }
        }

        public int lastAckPacketId
        {
            get { return m_lastAckPacketId; }
            set { m_lastAckPacketId = value; }
        }

        public int lastSentPacketId
        {
            get { return m_lastSentPacketId; }
            set { m_lastSentPacketId = value; }
        }

        public int networkPackageDiff
        {
            get { return m_lastSentPacketId - m_lastAckPacketId; }
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

        #region Helpers

        private Player[] GetTempArray(int size)
        {
            if (s_tempArray == null || s_tempArray.Length < size)
            {
                s_tempArray = new Player[size];
            }

            return s_tempArray;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Getters/Setters

        public bool IsAlive
        {
            get { return m_alive; }
            set { m_alive = value; }
        }

        public bool IsNetworkPlayer
        {
            get { return m_input is PlayerNetworkInput; }
        }

        public int GetIndex()
        {
            return m_index;
        }

        public bool IsReady
        {
            get { return m_ready; }
            set { m_ready = value; }
        }

        public float errDx
        {
            get { return m_errDx; }
        }

        public float errDy
        {
            get { return m_errDy; }
        }

        #endregion
    }
}

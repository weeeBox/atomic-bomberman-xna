using System;
using System.Collections.Generic;
using BomberEngine;
using Bomberman.Content;
using Bomberman.Gameplay.Elements.Fields;
using Bomberman.Gameplay.Elements.Players;

namespace Bomberman.Gameplay
{
    public class GameNotifications
    {
        public static readonly String RoundEnded     = "RoundEnded";
        public static readonly String RoundRestarted = "RoundRestarted";
        public static readonly String GameEnded      = "GameEnded";
    }

    public class Game : BaseObject, IResettable, IDestroyable
    {
        private PlayerList      m_players;
        private Field           m_field;
        private Scheme          m_currentScheme;
        private TimerManager    m_timerManager;

        private int m_roundIndex;
        private int m_totalRounds;

        public Game()
        {
            m_timerManager = new TimerManager();
            m_players = new PlayerList(m_timerManager, CVars.cg_maxPlayers.intValue);

            m_field = new Field(this);

            RegisterNotification(Notifications.ConsoleVariableChanged, ConsoleVariableChanged);

            Reset();
        }

        /// <summary>
        /// Mock constructor
        /// </summary>
        protected Game(int width, int height)
        {
            m_timerManager = new TimerManager();
            m_players = new PlayerList(m_timerManager, CVars.cg_maxPlayers.intValue);
            m_field = new Field(this, width, height);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region IResettable

        public void Reset()
        {
            m_timerManager.CancelAll(this);

            m_roundIndex = 0;
            m_totalRounds = CVars.roundsToWin.intValue;
            m_players.Clear();
            m_field.Reset();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IDestroyable

        public void Destroy()
        {
            UnregisterNotifications();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public void AddPlayer(Player player)
        {
            m_field.AddPlayer(player);
        }

        public PlayerList GetPlayers()
        {
            return m_players;
        }

        public List<Player> GetPlayersList()
        {
            return m_players.list;
        }

        public int GetPlayersCount()
        {
            return m_players.GetCount();
        }

        /* Loads field from scheme: setups bricks, powerups and players */
        public void LoadField(Scheme scheme)
        {
            m_currentScheme = scheme;
            m_field.Load(scheme);
        }

        /* Loads field from scheme: setups ONLY bricks */
        public void SetupField(Scheme scheme)
        {
            m_currentScheme = scheme;
            m_field.Setup(scheme);
        }

        public void Restart()
        {
            m_field.Restart(m_currentScheme);
            PostNotification(GameNotifications.RoundRestarted);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Round

        public void StartNextRound()
        {
            Assert.IsTrue(m_roundIndex < m_totalRounds - 1);
            ++m_roundIndex;

            Restart();
        }

        public void EndRound()
        {
            if (m_roundIndex < roundsCount - 1)
            {
                NotifyRoundEnded();
            }
            else
            {
                NotifyGameEnded();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Listener's notifications

        private void NotifyRoundEnded()
        {
            PostNotification(GameNotifications.RoundEnded);
        }

        private void NotifyGameEnded()
        {
            PostNotification(GameNotifications.GameEnded);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        private void ConsoleVariableChanged(Notification notification)
        {
            CVar cvar = notification.GetData<CVar>();
            if (cvar == CVars.roundsToWin)
            {
                int totalRounds = cvar.intValue;
                m_totalRounds = Math.Max(totalRounds, roundIndex + 1);
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public static Game Current
        {
            get { return BmRootController.Current.game; }
        }

        public TimerManager timerManager
        {
            get { return m_timerManager; }
        }

        public static PlayerList Players()
        {
            return Current.GetPlayers();
        }

        public Field Field
        {
            get { return m_field; }
        }

        public bool IsGameEnded
        {
            get { return m_roundIndex == m_totalRounds - 1; }
        }

        public int roundIndex
        {
            get { return m_roundIndex; }
            set { m_roundIndex = value; }
        }

        public int roundsCount
        {
            get { return m_totalRounds; }
            set { m_totalRounds = value; }
        }

        #endregion
    }
}

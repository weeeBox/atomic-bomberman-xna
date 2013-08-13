using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Bomberman.Game.Elements;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Fields;
using Bomberman.Content;
using BomberEngine.Debugging;
using BomberEngine.Game;

namespace Bomberman.Game
{
    public class GameNotifications
    {
        public static readonly String RoundEnded     = "RoundEnded";
        public static readonly String RoundRestarted = "RoundRestarted";
        public static readonly String GameEnded      = "GameEnded";
    }

    public enum MultiplayerMode
    {
        None,
        Client,
        Server,
    }

    public class Game : BaseObject
    {
        private static Game s_current;

        private Field m_field;
        private MultiplayerMode m_multiplayerMode;

        private Scheme m_currentScheme;
        private int m_roundIndex;

        public Game(MultiplayerMode mode)
        {
            s_current = this;

            m_multiplayerMode = mode;
            m_field = new Field(this);
        }

        public void AddPlayer(Player player)
        {
            m_field.AddPlayer(player);
        }

        public PlayerList GetPlayers()
        {
            return m_field.GetPlayers();
        }

        public List<Player> GetPlayersList()
        {
            return m_field.GetPlayers().list;
        }

        public int GetPlayersCount()
        {
            return m_field.GetPlayers().GetCount();
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
            PostNotificationImmediately(GameNotifications.RoundRestarted);
        }

        public void StartNextRound()
        {
            Debug.Assert(m_roundIndex < CVars.roundsToWin.intValue - 1);
            ++m_roundIndex;

            Restart();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Round

        public void EndRound()
        {
            if (m_roundIndex < CVars.roundsToWin.intValue - 1)
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

        #region Getters/Setters

        public static Game Current
        {
            get { return s_current; }
        }

        public static PlayerList Players()
        {
            return s_current.GetPlayers();
        }

        public Field Field
        {
            get { return m_field; }
        }

        public bool IsMuliplayerClient
        {
            get { return m_multiplayerMode == MultiplayerMode.Client; }
        }

        public bool IsMuliplayerServer
        {
            get { return m_multiplayerMode == MultiplayerMode.Server; }
        }

        public bool IsNetworkMultiplayer
        {
            get { return IsMuliplayerClient || IsMuliplayerServer; }
        }

        public bool IsLocal
        {
            get { return m_multiplayerMode == MultiplayerMode.None; }
        }

        #endregion
    }
}

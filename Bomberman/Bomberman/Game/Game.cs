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
        public static readonly String RoundEnded = "Game Ended";
        public static readonly String GameEnded  = "Round Ended";
    }

    public class Game
    {
        private static Game s_current;

        private Field m_field;
        private Scheme m_currentScheme;
        private int m_roundIndex;

        public Game()
        {
            s_current = this;
            m_field = new Field();
        }

        public void AddPlayer(Player player)
        {
            m_field.AddPlayer(player);
        }

        public PlayerList GetPlayers()
        {
            return m_field.GetPlayers();
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

        #region Helpers

        private void PostNotification(String name)
        {
            Application.Notifications().Post(name, this);
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

        #endregion
    }
}

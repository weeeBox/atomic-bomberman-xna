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

namespace Bomberman.Game
{
    public interface IGameListener
    {
        void OnRoundEnded(Game game);
        void OnGameEnded(Game game);
    }

    public class Game
    {
        public Field field;

        private static Game current;

        private Scheme currentScheme;

        private int roundIndex;

        public IGameListener listener;

        public Game()
        {
            current = this;
            field = new Field();
        }

        public void AddPlayer(Player player)
        {
            field.AddPlayer(player);
        }

        public PlayerList GetPlayers()
        {
            return field.GetPlayers();
        }

        public int GetPlayersCount()
        {
            return field.GetPlayers().GetCount();
        }

        /* Loads field from scheme: setups bricks, powerups and players */
        public void LoadField(Scheme scheme)
        {
            currentScheme = scheme;
            field.Load(scheme);
        }

        /* Loads field from scheme: setups ONLY bricks */
        public void SetupField(Scheme scheme)
        {
            currentScheme = scheme;
            field.Setup(scheme);
        }

        public void Restart()
        {
            field.Restart(currentScheme);
        }

        public void StartNextRound()
        {
            Debug.Assert(roundIndex < CVars.roundsToWin.intValue - 1);
            ++roundIndex;

            Restart();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Round

        public void EndRound()
        {
            ++roundIndex;
            if (roundIndex < CVars.roundsToWin.intValue)
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
            if (listener != null)
                listener.OnRoundEnded(this);
        }

        private void NotifyGameEnded()
        {
            if (listener != null)
                listener.OnGameEnded(this);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Getters/Setters

        public static PlayerList Players()
        {
            return current.GetPlayers();
        }

        public static Field Field()
        {
            return current.field;
        }

        public static Game Current()
        {
            return current;
        }

        #endregion
    }
}

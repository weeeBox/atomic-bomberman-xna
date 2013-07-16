using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Screens;
using BombermanCommon.Resources;
using Bomberman.Content;
using Assets;

namespace Bomberman.Game
{
    public class GameLobbyController : BombermanController
    {
        public enum ExitCode
        {
            StartGame,
            Cancel
        }

        private Scheme selectedScheme;

        public GameLobbyController()
        {   
        }

        protected override void OnStart()
        {
            selectedScheme = Assets().GetScheme(A.maps_x);
            StartScreen(new PlayersScreen(selectedScheme));
        }

        private void Stop(ExitCode code)
        {
            Stop((int)code);
        }
    }
}

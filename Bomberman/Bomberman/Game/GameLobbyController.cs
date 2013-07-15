using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Screens;

namespace Bomberman.Game
{
    public class GameLobbyController : BombermanController
    {
        public enum ExitCode
        {
            StartGame,
            Cancel
        }

        public GameLobbyController()
        {

        }

        protected override void OnStart()
        {
            StartScreen(new SchemeScreen());
        }

        private void Stop(ExitCode code)
        {
            Stop((int)code);
        }
    }
}

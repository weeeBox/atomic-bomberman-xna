using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using Bomberman.Game;

namespace Bomberman
{
    public class BombermanRootController : RootController
    {
        private GameController gameController;

        public BombermanRootController()
        {
            gameController = new GameController();
        }

        protected override void OnStart()
        {
            StartController(gameController);
        }
    }
}

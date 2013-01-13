using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using Bomberman.Game.Scenes;

namespace Bomberman.Game
{
    public class GameController : Controller
    {
        private GameScene gameScene;

        public GameController()
        {
            gameScene = new GameScene();
        }

        protected override void OnStart()
        {
            StartScene(gameScene);
        }
    }
}

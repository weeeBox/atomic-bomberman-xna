using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using Bomberman.Game;
using Assets;

namespace Bomberman
{
    public class BombermanRootController : RootController
    {
        private GameController gameController;

        public BombermanRootController()
        {   
        }

        protected override void OnStart()
        {
            BombermanAssetManager manager = (BombermanAssetManager) Application.Assets();
            manager.AddPackToLoad(AssetPacks.Packs.ALL);
            manager.LoadImmediately();

            gameController = new GameController();

            StartController(gameController);
        }
    }
}

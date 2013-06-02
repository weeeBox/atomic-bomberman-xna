﻿using Assets;
using BomberEngine.Debugging;
using BomberEngine.Game;
using Bomberman.Game;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Bomberman.Menu;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Assets.Types;
using BomberEngine.Core.Input;
using BomberEngine.Consoles;
using Bomberman.Game.Elements.Players;

namespace Bomberman
{
    public class BombermanRootController : RootController
    {
        private MenuController menuController;

        public BombermanRootController(ContentManager contentManager)
            : base(contentManager)
        {   
        }

        protected override void OnStart()
        {
            BombermanAssetManager manager = (BombermanAssetManager)Application.Assets();
            manager.AddPackToLoad(AssetPacks.Packs.ALL);
            manager.LoadImmediately();

            menuController = new MenuController();
            StartController(menuController);
        }

        protected override void OnControllerStop(Controller controller)
        {
            switch (controller.exitCode)
            {
                case ExitCode.StartGame:
                    StartController(new GameController("x"));
                    break;

                case ExitCode.Quit:
                    Application.sharedApplication.RunStop();
                    break;
            }
        }

        protected override CConsole CreateConsole()
        {
            CConsole console = base.CreateConsole();
            CVars.Register(console);
            return console;
        }

        public override bool OnKeyPressed(KeyEventArg e)
        {
            if (base.OnKeyPressed(e))
            {
                return true;
            }

            if (e.key == KeyCode.Oem8)
            {
                ToggleConsole();
                return true;
            }

            return false;
        }
    }
}

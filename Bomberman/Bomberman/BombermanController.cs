using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using Bomberman.Multiplayer;
using BomberEngine.Core.Input;

namespace Bomberman
{
    public abstract class BombermanController : Controller
    {
        protected new BombermanRootController GetRootController()
        {
            return base.GetRootController() as BombermanRootController;
        }

        protected MultiplayerManager GetMultiplayerManager()
        {
            return GetRootController().GetMultiplayerManager();
        }

        protected BombermanAssetManager Assets()
        {
            return BombermanApplication.Assets();
        }

        protected InputManager Input()
        {
            return BombermanApplication.Input();
        }
    }
}

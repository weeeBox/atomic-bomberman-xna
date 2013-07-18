using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using Bomberman.Multiplayer;
using BomberEngine.Core.Input;

namespace Bomberman
{
    public abstract class BmController : Controller
    {
        protected new BmRootController GetRootController()
        {
            return base.GetRootController() as BmRootController;
        }

        protected MultiplayerManager GetMultiplayerManager()
        {
            return GetRootController().GetMultiplayerManager();
        }

        protected BmAssetManager Assets()
        {
            return BmApplication.Assets();
        }

        protected InputManager Input()
        {
            return BmApplication.Input();
        }
    }
}

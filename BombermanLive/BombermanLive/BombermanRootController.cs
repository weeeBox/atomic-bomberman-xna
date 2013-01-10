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
        public BombermanRootController()
        {
        }

        protected override void OnStart()
        {
            StartController(new StartupController());
        }
    }
}

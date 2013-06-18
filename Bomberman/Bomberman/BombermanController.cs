using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;

namespace Bomberman
{
    public abstract class BombermanController : Controller
    {
        protected override BombermanRootController GetRootController()
        {
            return base.GetRootController() as BombermanRootController;
        }
    }
}

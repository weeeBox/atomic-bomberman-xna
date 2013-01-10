using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using Microsoft.Xna.Framework;
using Bomberman.Game;

namespace Bomberman
{
    public class BombermanApplication : Application
    {
        public BombermanApplication(GraphicsDeviceManager graphics) : base(graphics)
        {
            RootController = new BombermanRootController();
        }
    }
}

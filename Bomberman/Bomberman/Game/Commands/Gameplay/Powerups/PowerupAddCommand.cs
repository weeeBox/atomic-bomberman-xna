using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging.Commands;
using BomberEngine.Debugging;

namespace Bomberman.Game.Commands.Gameplay.Powerups
{
    public class PowerupAddCommand : ConsoleCommand
    {
        public PowerupAddCommand()
            : base("powerup_add")
        {
        }

        public override void Execute(GameConsole console, params String[] args)
        {
            throw new NotImplementedException();
        }
    }
}

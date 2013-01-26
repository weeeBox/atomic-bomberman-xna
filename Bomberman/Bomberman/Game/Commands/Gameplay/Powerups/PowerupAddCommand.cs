using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging.Commands;

namespace Bomberman.Game.Commands.Gameplay.Powerups
{
    public class PowerupAddCommand : ConsoleCommand
    {
        public PowerupAddCommand()
            : base("powerup_add")
        {
        }

        public override void Execute(Dictionary<string, string> prms)
        {
            
        }
    }
}

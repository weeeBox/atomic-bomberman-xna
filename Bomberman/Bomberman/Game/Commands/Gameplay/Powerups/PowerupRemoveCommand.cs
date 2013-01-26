using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging.Commands;

namespace Bomberman.Game.Commands.Gameplay.Powerups
{
    public class PowerupRemoveCommand : ConsoleCommand
    {
        public PowerupRemoveCommand()
            : base("powerup_remove")
        {
        }

        public override void Execute(Dictionary<string, string> prms)
        {   
        }
    }
}

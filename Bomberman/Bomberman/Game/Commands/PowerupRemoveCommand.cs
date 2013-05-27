using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Consoles.Commands;
using BomberEngine.Debugging;
using BomberEngine.Consoles;

namespace Bomberman.Game.Commands
{
    public class PowerupRemoveCommand : IngameConsoleCommand
    {
        public PowerupRemoveCommand()
            : base("powerup_remove")
        {
        }

        public override void Execute(params String[] args)
        {   
        }
    }
}

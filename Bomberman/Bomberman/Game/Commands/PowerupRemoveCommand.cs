using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Consoles.Commands;
using BomberEngine.Debugging;
using BomberEngine.Consoles;

namespace Bomberman.Game.Commands
{
    public class PowerupRemoveCommand : BombermanConsoleCommand
    {
        public PowerupRemoveCommand()
            : base("powerup_remove")
        {
        }

        public override void Execute(GameConsole console, params String[] args)
        {   
        }
    }
}

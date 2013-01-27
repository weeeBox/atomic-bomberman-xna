using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging.Commands;
using BomberEngine.Debugging;

namespace Bomberman.Game.Commands.Gameplay.Powerups
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

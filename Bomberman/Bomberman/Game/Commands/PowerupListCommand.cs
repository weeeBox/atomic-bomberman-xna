using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Consoles.Commands;
using BomberEngine.Debugging;
using BomberEngine.Consoles;

namespace Bomberman.Game.Commands
{
    public class PowerupListCommand : ConsoleCommand
    {
        public PowerupListCommand()
            : base("powerup_list")
        {
        }

        public override void Execute(GameConsole console, params string[] args)
        {
            String[] names = Bomberman.Game.Elements.Powerups.Names();
            console.AddLine("Powerups:");

            for (int i = 0; i < names.Length; ++i)
            {
                console.AddLine(" " + i + ": " + names[i]);
            }
        }
    }
}

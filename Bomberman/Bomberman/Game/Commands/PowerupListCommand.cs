using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Consoles.Commands;
using BomberEngine.Debugging;
using BomberEngine.Consoles;

namespace Bomberman.Game.Commands
{
    public class PowerupListCommand : CCommand
    {
        public PowerupListCommand()
            : base("powerup_list")
        {
        }

        public override void Execute(params String[] args)
        {
            String[] names = Bomberman.Game.Elements.Powerups.Names();
            Print("Powerups:");

            for (int i = 0; i < names.Length; ++i)
            {
                Print(" " + i + ": " + names[i]);
            }
        }
    }
}

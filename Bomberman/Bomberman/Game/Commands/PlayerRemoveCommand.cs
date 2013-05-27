using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Consoles.Commands;
using BomberEngine.Debugging;
using BomberEngine.Consoles;

namespace Bomberman.Game.Commands
{
    public class PlayerRemoveCommand : IngameConsoleCommand
    {
        public PlayerRemoveCommand()
            : base("player_remove")
        {
        }

        public override void Execute(SystemConsole console, params String[] args)
        {
            throw new NotImplementedException();
        }
    }
}

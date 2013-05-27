using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Consoles.Commands;
using BomberEngine.Debugging;
using BomberEngine.Consoles;

namespace Bomberman.Game.Commands
{
    public class PlayerAddCommand : BombermanConsoleCommand
    {
        public PlayerAddCommand()
            : base("player_add")
        {
        }

        public override void Execute(GameConsole console, params String[] args)
        {
            throw new NotImplementedException();
        }
    }
}

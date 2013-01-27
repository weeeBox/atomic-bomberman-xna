using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging.Commands;
using BomberEngine.Debugging;

namespace Bomberman.Game.Commands.Gameplay.Players
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

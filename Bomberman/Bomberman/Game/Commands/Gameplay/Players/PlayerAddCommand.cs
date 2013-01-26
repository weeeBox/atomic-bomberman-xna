using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging.Commands;

namespace Bomberman.Game.Commands.Gameplay.Players
{
    public class PlayerAddCommand : ConsoleCommand
    {
        public PlayerAddCommand()
            : base("player_add")
        {
        }

        public override void Execute(params string[] args)
        {   
        }
    }
}

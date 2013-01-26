﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging.Commands;
using BomberEngine.Debugging;

namespace Bomberman.Game.Commands.Gameplay.Players
{
    public class PlayerRemoveCommand : ConsoleCommand
    {
        public PlayerRemoveCommand()
            : base("player_remove")
        {
        }

        public override void Execute(GameConsole console, params String[] args)
        {
            throw new NotImplementedException();
        }
    }
}

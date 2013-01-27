using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging.Commands;
using BomberEngine.Debugging;
using Bomberman.Game.Elements.Players;

namespace Bomberman.Game.Commands.Gameplay.Powerups
{
    public class PowerupAddCommand : BombermanConsoleCommand
    {
        public PowerupAddCommand()
            : base("powerup_add")
        {
        }

        public override void Execute(GameConsole console, params String[] args)
        {
            List<Player> players = GetPlayers();
            int playerIndex = GetInt(args, "-p");

            if (playerIndex < players.Count)
            {
                Player player = players[playerIndex];
            }
            else
            {
                Log(console, "Player index out of bounds: " + playerIndex);
            }
        }
    }
}

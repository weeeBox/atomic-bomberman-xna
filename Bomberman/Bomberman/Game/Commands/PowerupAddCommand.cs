using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Consoles.Commands;
using BomberEngine.Debugging;
using Bomberman.Game.Elements.Players;
using BomberEngine.Consoles;

namespace Bomberman.Game.Commands
{
    public class PowerupAddCommand : IngameConsoleCommand
    {
        public PowerupAddCommand()
            : base("powerup_add")
        {
        }

        public override void Execute(params String[] args)
        {
            List<Player> players = GetPlayers();
            Player player = players[0];

            int index = GetInt(args, 0, -1);
            if (index != -1)
            {
                player.TryAddPowerup(index);
            }
            else
            {
                Print("Wrong powerup index");
            }
        }
    }
}

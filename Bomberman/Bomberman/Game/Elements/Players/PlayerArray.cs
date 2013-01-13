using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Game.Elements.Players
{
    public class PlayerArray
    {
        private List<Player> players;

        public PlayerArray()
        {
            players = new List<Player>();
        }

        public void Add(Player player)
        {
            players.Add(player);
        }

        public List<Player> GetList()
        {
            return players;
        }
    }
}

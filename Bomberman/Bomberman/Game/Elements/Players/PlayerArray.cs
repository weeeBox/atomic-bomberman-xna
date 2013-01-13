using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;

namespace Bomberman.Game.Elements.Players
{
    public class PlayerArray : Updatable
    {
        private List<Player> players;

        public PlayerArray()
        {
            players = new List<Player>();
        }

        public void Update(float delta)
        {
            foreach (Player player in players)
            {
                player.Update(delta);
            }
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

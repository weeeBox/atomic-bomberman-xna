using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;

namespace Bomberman.Game.Elements.Players
{
    public class PlayerList : Updatable
    {
        public List<Player> list;

        public PlayerList()
        {
            list = new List<Player>();
        }

        public void Update(float delta)
        {
            foreach (Player player in list)
            {
                player.Update(delta);
            }
        }

        public void Add(Player player)
        {
            list.Add(player);
        }

        public Player TryGet(int playerIndex)
        {
            if (playerIndex >= 0 && playerIndex < list.Count)
            {
                return list[playerIndex];
            }

            return null;
        }

        public Player Get(int playerIndex)
        {
            return list[playerIndex];
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Bomberman.Game.Elements;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Fields;
using Bomberman.Content;

namespace Bomberman.Game
{
    public class Game
    {
        private PlayerList players;
        private Match match;
        private Field field;

        private static Game current;

        public Game()
        {
            current = this;
            players = new PlayerList();
        }

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public List<Player> GetPlayers()
        {
            return players.list;
        }

        public void LoadField(Scheme scheme)
        {
            field = new Field(scheme, players);
        }

        public static PlayerList Players()
        {
            return current.players;
        }

        public static Match Match()
        {
            return current.match;
        }

        public static Field Field()
        {
            return current.field;
        }
    }
}

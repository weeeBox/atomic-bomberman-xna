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
        private Match match;
        private Field field;

        private static Game current;

        public Game()
        {
            current = this;
            field = new Field();
        }

        public void AddPlayer(Player player)
        {
            field.AddPlayer(player);
        }

        public PlayerList GetPlayers()
        {
            return field.GetPlayers();
        }

        public void LoadField(Scheme scheme)
        {
            field.Setup(scheme);
        }

        public static PlayerList Players()
        {
            return current.GetPlayers();
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

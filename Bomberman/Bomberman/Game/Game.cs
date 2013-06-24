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
        public Field field;

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

        public int GetPlayersCount()
        {
            return field.GetPlayers().GetCount();
        }

        /* Loads field from scheme: setups bricks, powerups and players */
        public void LoadField(Scheme scheme)
        {
            field.Load(scheme);
        }

        /* Loads field from scheme: setups ONLY bricks */
        public void SetupField(Scheme scheme)
        {
            field.Setup(scheme);
        }

        public static PlayerList Players()
        {
            return current.GetPlayers();
        }

        public static Field Field()
        {
            return current.field;
        }
    }
}

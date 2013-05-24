using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements;

namespace Bomberman.Game
{
    public class GameCheats : IKeyboardListener
    {
        private Game game;

        private Dictionary<Keys, int> lookup;

        public GameCheats(Game game)
        {
            this.game = game;

            lookup = new Dictionary<Keys, int>();
            
            lookup[Keys.D1] = Powerups.Bomb;
            lookup[Keys.D2] = Powerups.Flame;
            lookup[Keys.D4] = Powerups.Kick;
            lookup[Keys.D5] = Powerups.Speed;
            lookup[Keys.D6] = Powerups.Punch;
            lookup[Keys.D7] = Powerups.Grab;
            lookup[Keys.D8] = Powerups.Spooger;
            lookup[Keys.D9] = Powerups.GoldFlame;
            lookup[Keys.D0] = Powerups.Trigger;
            lookup[Keys.OemMinus] = Powerups.Jelly;
        }

        public bool OnKeyPressed(Keys key)
        {
            List<Player> players = game.GetPlayers().list;
            Player player = players[0];

            if (lookup.ContainsKey(key))
            {
                int powerupIndex = lookup[key];
                player.TryAddPowerup(powerupIndex);
                return true;
            }

            return false;
        }

        public bool OnKeyRepeated(Keys key)
        {
            return false;
        }

        public bool OnKeyReleased(Keys key)
        {
            return false;
        }
    }
}

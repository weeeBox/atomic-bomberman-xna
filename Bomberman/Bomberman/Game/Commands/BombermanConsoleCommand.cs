using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Consoles.Commands;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements.Players;

namespace Bomberman.Game.Commands
{
    public abstract class BombermanConsoleCommand : ConsoleCommand
    {
        public BombermanConsoleCommand(String name)
            : base(name)
        {
        }

        protected Field GetField()
        {
            return Field.Current();
        }

        protected List<Player> GetPlayers()
        {
            return GetField().GetPlayers().list;
        }

        protected Player GetPlayer(int index)
        {
            List<Player> list = GetPlayers();
            if (index >= 0 && index < list.Count)
            {
                return list[index];
            }
            return null;
        }
    }
}

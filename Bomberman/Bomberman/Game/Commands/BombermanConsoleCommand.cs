using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging.Commands;
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
    }
}

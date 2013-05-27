using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Consoles.Commands;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements.Players;
using BomberEngine.Consoles;
using Bomberman.Game.Elements;

namespace Bomberman.Game
{
    public class GameCommandList : CCommandList
    {
        protected override CCommand[] CreateCommands()
        {
            return new CCommand[]
            {
                new Cmd_infect(),
                new Cmd_add(),
            };
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public abstract class GameCCommand : CCommand
    {
        public GameCCommand(String name)
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

    //////////////////////////////////////////////////////////////////////////////

    class Cmd_infect : GameCCommand
    {
        public Cmd_infect()
            : base("infect")
        {
        }

        public override void Execute()
        {
            if (ArgsCount() == 1)
            {
                int diseaseIndex = IntArg(0);

                Diseases disease = Diseases.FromIndex(diseaseIndex);
                if (disease != null)
                {
                    Player player = GetPlayer(0);
                    bool infected = player.TryInfect(diseaseIndex);
                    if (infected)
                    {
                        Print("Infected: " + Diseases.FromIndex(diseaseIndex).name);
                    }
                }
            }
            else
            {
                Print("usage: " + name + " <index>");
                Diseases[] array = Diseases.array;
                for (int i = 0; i < array.Length; ++i)
                {
                    PrintIndent(i + ": " + array[i].name);
                }
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class Cmd_add : GameCCommand
    {
        public Cmd_add()
            : base("add")
        {
        }

        public override void Execute()
        {
            List<Player> players = GetPlayers();
            Player player = players[0];

            int index = IntArg(0, -1);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Consoles;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements;
using BomberEngine.Game;

namespace Bomberman.Game
{
    public delegate void ExecuteDelegate();

    public abstract class GameCCommand : CCommand
    {
        public GameCCommand(String name)
            : base(name)
        {
        }

        protected GameController GetGameController()
        {
            return GetCurrentController() as GameController;
        }

        protected Controller GetCurrentController()
        {
            return Application.RootController().GetCurrentController();
        }

        protected Game GetGame()
        {
            return Game.Current;
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

                Disease disease = DiseaseList.DiseaseForIndex(diseaseIndex);
                if (disease != null)
                {
                    Player player = GetPlayer(0);
                    bool infected = player.TryInfect(diseaseIndex);
                    if (infected)
                    {
                        Print("Infected: " + disease.name);
                    }
                }
            }
            else
            {
                Print("usage: " + name + " <index>");
                Disease[] array = DiseaseList.diseaseArray;
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
                Print("Powerups:");
                for (int i = 0; i < Powerups.Count; ++i)
                {
                    Print(i + ":" + Powerups.Name(i));
                }
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class Cmd_map : GameCCommand
    {
        public Cmd_map()
            : base("map")
        {
        }

        public override void Execute()
        {
            if (ArgsCount() != 1)
            {
                Print("usage: " + name + " <map>");
                return;
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class Cmd_map_restart : GameCCommand
    {
        public Cmd_map_restart()
            : base("map_restart")
        {
        }

        public override void Execute()
        {
            GameController gc = GetGameController();
            if (gc != null)
            {
                gc.Restart();
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class Cmd_test : GameCCommand
    {
        private ExecuteDelegate m_delegate;

        public Cmd_test(String name, ExecuteDelegate del)
            : base(name)
        {
            m_delegate = del;
        }

        public override void Execute()
        {
            if (m_delegate != null)
            {
                m_delegate();
            }
        }
    }
}

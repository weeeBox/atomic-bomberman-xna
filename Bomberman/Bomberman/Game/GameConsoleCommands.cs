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
    public abstract class IngameConsoleCommand : CCommand
    {
        public IngameConsoleCommand(String name)
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

    class CInfect : IngameConsoleCommand
    {
        public CInfect()
            : base("infect")
        {
        }

        public override void Execute(params String[] args)
        {
            int diseaseIndex = GetInt(args, 0);

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
    }
}

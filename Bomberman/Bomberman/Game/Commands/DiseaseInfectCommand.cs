using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;
using Bomberman.Game.Elements;
using Bomberman.Game.Elements.Players;
using BomberEngine.Consoles;

namespace Bomberman.Game.Commands
{
    public class DiseaseInfectCommand : IngameConsoleCommand
    {
        public DiseaseInfectCommand()
            : base("disease_infect")
        {
        }

        public override void Execute(SystemConsole console, params String[] args)
        {
            int diseaseIndex = GetInt(args, 0);

            Diseases disease = Diseases.FromIndex(diseaseIndex);
            if (disease != null)
            {
                Player player = GetPlayer(0);
                bool infected = player.TryInfect(diseaseIndex);
                if (infected)
                {
                    console.AddLine("Infected: " + Diseases.FromIndex(diseaseIndex).name);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;
using Bomberman.Game.Elements;
using BomberEngine.Consoles;

namespace Bomberman.Game.Commands
{
    public class DiseaseListCommand : IngameConsoleCommand
    {
        public DiseaseListCommand()
            : base("disease_list")
        {
        }

        public override void Execute(params String[] args)
        {
            Print("Diseases:");
            Diseases[] array = Diseases.array;
            for (int i = 0; i < array.Length; ++i)
            {
                Print(i + ": " + array[i].name);
            }
        }
    }
}

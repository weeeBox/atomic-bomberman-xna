using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;
using Bomberman.Game.Elements;
using BomberEngine.Consoles;

namespace Bomberman.Game.Commands
{
    public class DiseaseListCommand : BombermanConsoleCommand
    {
        public DiseaseListCommand()
            : base("disease_list")
        {
        }

        public override void Execute(GameConsole console, params String[] args)
        {
            Log(console, "Diseases:");
            Diseases[] array = Diseases.array;
            for (int i = 0; i < array.Length; ++i)
            {
                Log(console, i + ": " + array[i].name);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;

namespace BomberEngine.Consoles.Commands
{
    public class ExitCommand : CCommand
    {
        public ExitCommand()
            : base("exit")
        {
        }

        public override void Execute(params String[] args)
        {
            Application.sharedApplication.Stop();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;

namespace BomberEngine.Consoles.Commands
{
    public class Cmd_listcmds : CCommand
    {
        public Cmd_listcmds()
            : base("listcmds")
        {
        }

        public override void Execute()
        {
            List<CCommand> commands = console.ListCommands();
            commands.Sort(CompareCommands);

            for (int i = 0; i < commands.Count; ++i)
            {
                Print(commands[i].name);
            }
            Print(commands.Count + " commands");
        }

        private int CompareCommands(CCommand a, CCommand b)
        {
            return a.name.CompareTo(b.name);
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class Cmd_listcvars : CCommand
    {
        public Cmd_listcvars()
            : base("listcvars")
        {
        }

        public override void Execute()
        {
            Print("Not implemented yet");
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class Cmd_exit : CCommand
    {
        public Cmd_exit()
            : base("exit")
        {
        }

        public override void Execute()
        {
            Application.sharedApplication.Stop();
        }
    }
}

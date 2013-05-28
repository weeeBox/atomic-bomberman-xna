using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Consoles;
using BomberEngine.Core.IO;

namespace BomberEngine.Game
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

    //////////////////////////////////////////////////////////////////////////////

    public class Cmd_exec : CCommand
    {
        public Cmd_exec()
            : base("exec")
        {
        }

        public override void Execute()
        {
            if (ArgsCount() != 1)
            {
                Print("usage: " + name + " <filename>");
                return;
            }

            String filename = StrArg(0);
            List<String> lines = File.Read(filename);

            if (lines == null)
            {
                Print("Can't read file: " + filename);
                return;
            }

            foreach (String line in lines)
            {
                String trim = line.Trim();
                if (trim.Length == 0 || trim.StartsWith("//"))
                {
                    continue;
                }

                console.TryExecuteCommand(line);
            }
        }
    }
}

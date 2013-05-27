using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Consoles.Commands
{
    public abstract class CCommandList
    {
        private CCommand[] commands;

        public CCommandList()
        {
            commands = CreateCommands();
        }

        public void Register()
        {
            CConsole console = Console();
            for (int i = 0; i < commands.Length; ++i)
            {
                console.RegisterCommand(commands[i]);
            }
        }

        public void Unregister()
        {
            CConsole console = Console();
            for (int i = 0; i < commands.Length; ++i)
            {
                console.UnregisterCommand(commands[i]);
            }
        }

        protected CConsole Console()
        {
            return CConsole.Current();
        }

        protected abstract CCommand[] CreateCommands();
    }
}

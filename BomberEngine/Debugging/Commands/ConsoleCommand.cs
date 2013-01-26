using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Debugging.Commands
{
    public abstract class ConsoleCommand
    {
        private String name;

        protected ConsoleCommand(String name)
        {
            this.name = name;
        }

        public abstract void Execute(Dictionary<String, String> prms);

        public String GetName()
        {
            return name;
        }
    }
}

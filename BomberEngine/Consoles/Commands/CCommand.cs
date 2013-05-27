using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Consoles.Commands
{
    public abstract class CCommand
    {
        public String name;
        public Cmd console;
        public String args;

        protected CCommand(String name)
        {
            this.name = name;
        }

        public abstract void Execute(params String[] args);

        protected int GetInt(String[] args, String param)
        {
            return GetInt(args, param, 0);
        }

        protected int GetInt(String[] args, String param, int defValue)
        {
            for (int i = 0; i < args.Length - 1; ++i)
            {
                if (param.Equals(args[i]))
                {
                    return GetInt(args, i + 1, defValue);
                }
            }

            return defValue;
        }

        protected int GetInt(String[] args, int index)
        {
            return GetInt(args, index, 0);
        }

        protected int GetInt(String[] args, int index, int defValue)
        {
            String str = Get(args, index);
            if (str != null)
            {
                int value;
                bool succeed = int.TryParse(str, out value);
                return succeed ? value : defValue;
            }

            return defValue;
        }

        protected String Get(String[] args, int index)
        {
            return Get(args, index, null);
        }

        protected String Get(String[] args, int index, String defValue)
        {
            if (index >= 0 && index < args.Length)
            {
                return args[index];
            }

            return defValue;
        }

        protected void Print(String message)
        {
            console.Print(message);
        }

        protected void Print(String format, params Object[] args)
        {
            console.Print(format, args);
        }
    }
}

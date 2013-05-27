using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Consoles.Commands
{
    public abstract class CCommand
    {
        public CConsole console;

        public String name;
        public String[] args;

        protected CCommand(String name)
        {
            this.name = name;
        }

        public abstract void Execute();

        protected int IntArg(String param)
        {
            return IntArg(param, 0);
        }

        protected int IntArg(String name, int defValue)
        {
            for (int i = 1; i < args.Length - 1; ++i)
            {
                if (name.Equals(args[i]))
                {
                    return IntArg(i + 1, defValue);
                }
            }

            return defValue;
        }

        protected int IntArg(int index)
        {
            return IntArg(index, 0);
        }

        protected int IntArg(int index, int defValue)
        {
            String str = StrArg(index);
            if (str != null)
            {
                int value;
                bool succeed = int.TryParse(str, out value);
                return succeed ? value : defValue;
            }

            return defValue;
        }

        protected String StrArg(int index)
        {
            return StrArg(index, null);
        }

        protected String StrArg(int index, String defValue)
        {
            if (index >= 0 && index < args.Length - 1)
            {
                return args[index + 1];
            }

            return defValue;
        }

        protected String StrArg(String name, String defValue)
        {
            for (int i = 1; i < args.Length - 1; ++i)
            {
                if (name.Equals(args[i]))
                {
                    return StrArg(i + 1, defValue);
                }
            }

            return defValue;
        }

        protected int ArgsCount()
        {
            return args.Length - 1;
        }

        //////////////////////////////////////////////////////////////////////////////

        protected void Print(String message)
        {
            console.Print(message);
        }

        protected void Print(String format, params Object[] args)
        {
            console.Print(format, args);
        }

        protected void PrintIndent(String message)
        {
            console.PrintIndent(message);
        }

        protected void PrintIndent(String format, params Object[] args)
        {
            console.PrintIndent(format, args);
        }
    }
}

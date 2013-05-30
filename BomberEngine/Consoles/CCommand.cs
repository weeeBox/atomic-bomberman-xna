using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Consoles
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

        //////////////////////////////////////////////////////////////////////////////

        #region Args

        protected int IntArg(int index)
        {
            return IntArg(index, 0);
        }

        protected int IntArg(int index, int defValue)
        {
            String str = StrArg(index);
            return StringUtils.ParseInt(str, defValue);
        }

        protected int IntArg(int index, OutResult result)
        {
            String str = StrArg(index);
            return StringUtils.ParseInt(str, result);
        }

        protected float FloatArg(int index)
        {
            return FloatArg(index, 0.0f);
        }

        protected float FloatArg(int index, float defValue)
        {
            String str = StrArg(index);
            return StringUtils.ParseFloat(str, defValue);
        }

        protected float FloatArg(int index, OutResult result)
        {
            String str = StrArg(index);
            return StringUtils.ParseFloat(str, result);
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

        protected int ArgsCount()
        {
            return args.Length - 1;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Printing

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

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        public bool StartsWith(String prefix)
        {
            if (prefix.Length <= name.Length)
            {
                for (int i = 0; i < prefix.Length; ++i)
                {
                    char pc = char.ToLower(prefix[i]);
                    char nc = char.ToLower(name[i]);

                    if (pc != nc) return false;
                }

                return true;
            }

            return false;
        }

        #endregion

        #endregion
    }
}

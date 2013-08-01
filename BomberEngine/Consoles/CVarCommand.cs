using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Consoles
{
    public class CVarCommand : CCommand
    {
        public CVar cvar;

        public CVarCommand(CVar cvar)
            : base(cvar.name)
        {   
            this.cvar = cvar;
        }

        public override void Execute()
        {
            if (ArgsCount() == 0)
            {
                Print("\"" + cvar.name + "\" is:\"" + cvar.value + "\" default:\"" + cvar.defaultValue + "\"");
                return;
            }

            if (ArgsCount() != 1)
            {
                Print("usage ");
                return;
            }

            if (cvar.IsFloat())
            {
                OutResult result = OutResult.instance;

                float value = FloatArg(0, result);
                if (result.succeed)
                {
                    cvar.SetValue(value);
                    TryUpdateConfig();
                }
                else
                {
                    Print("Can't set float value");
                }
            }
            else if (cvar.IsInt())
            {
                OutResult result = OutResult.instance;

                int value = IntArg(0, result);
                if (result.succeed)
                {
                    cvar.SetValue(value);
                    TryUpdateConfig();
                }
                else
                {
                    Print("Can't set int value");
                }
            }
            else
            {
                String value = StrArg(0);
                cvar.SetValue(value);
                TryUpdateConfig();
            }
        }

        private bool TryUpdateConfig()
        {
            if (manual)
            {
                console.ScheduleConfigUpdate();
                return true;
            }

            return false;
        }
    }
}

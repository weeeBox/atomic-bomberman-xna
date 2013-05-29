using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Consoles
{
    public class CVarCommand : CCommand
    {
        private CVar cvar;

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
                float value = FloatArg(0, float.NaN);
                cvar.SetValue(value);
            }
        }
    }
}

using System;

namespace BomberEngine
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
                    SetValue(value);
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
                    SetValue(value);
                }
                else
                {
                    Print("Can't set int value");
                }
            }
            else
            {
                String value = StrArg(0);
                SetValue(value);
            }
        }

        public void SetValue(float value)
        {
            if (cvar.floatValue != value)
            {
                cvar.SetValue(value);
                TryScheduleConfigChanges();
            }
        }

        public void SetValue(int value)
        {
            if (cvar.intValue != value)
            {
                cvar.SetValue(value);
                TryScheduleConfigChanges();
            }
        }

        public void SetValue(String value)
        {
            if (cvar.value != value)
            {
                cvar.SetValue(value);
                TryScheduleConfigChanges();
            }
        }

        private bool TryScheduleConfigChanges()
        {
            if (manual)
            {
                if (!cvar.HasFlag(CFlags.DontSave))
                {
                    console.ScheduleConfigUpdate();
                }
                
                PostNotification(Notifications.ConsoleVariableChanged, cvar);
                return true;
            }

            return false;
        }

        public String value
        {
            get { return cvar.value; }
        }

        public String defaultValue
        {
            get { return cvar.defaultValue; }
        }

        public int intValue
        {
            get { return cvar.intValue; }
        }

        public float floatValue
        {
            get { return cvar.floatValue; }
        }

        public bool boolValue
        {
            get { return cvar.boolValue; }
        }

        public bool IsString()
        {
            return cvar.IsString();
        }

        public bool IsInt()
        {
            return cvar.IsInt();
        }

        public bool IsFloat()
        {
            return cvar.IsFloat();
        }

        public bool IsDefault()
        {
            return cvar.IsDefault();
        }

        public bool HasFlag(CFlags flag)
        {
            return cvar.HasFlag(flag);
        }
    }
}

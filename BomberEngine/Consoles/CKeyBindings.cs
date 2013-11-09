using System;
using System.Collections.Generic;

namespace BomberEngine
{
    public struct CKeyBinding
    {
        public String name;
        public String cmd;

        public CKeyBinding(String name, String cmd)
        {
            this.name = name;
            this.cmd = cmd;
        }
    }

    public class CKeyBindings : IKeyInputListener
    {
        private String[] bindings;
        private int bindingsCount;
        
        public CKeyBindings()
        {
            bindings = new String[(int)KeyCode.Count];
        }

        public void Bind(KeyCode code, String cmd)
        {
            int index = (int)code;
            if (bindings[index] == null)
            {
                ++bindingsCount;
                ScheduleConfigUpdate();
            }

            bindings[index] = cmd;
        }

        public void Unbind(KeyCode code)
        {
            int index = (int)code;
            if (bindings[index] != null)
            {
                Assert.True(bindingsCount > 0);
                --bindingsCount;
                ScheduleConfigUpdate();
            }
            bindings[index] = null;
        }

        public void UnbindAll()
        {
            ArrayUtils.Clear(bindings);
            bindingsCount = 0;
            ScheduleConfigUpdate();
        }

        public String FindCmd(KeyCode code)
        {
            return bindings[(int)code];
        }

        public List<CKeyBinding> ListBindings()
        {            
            List<CKeyBinding> list = new List<CKeyBinding>(bindingsCount);
            for (int i = 0, j = 0; i < bindings.Length && j < bindingsCount; ++i)
            {
                String cmd = bindings[i];
                if (cmd != null)
                {
                    KeyCode code = (KeyCode)i;
                    String name = KeyCodeHelper.ToString(code);
                    list.Add(new CKeyBinding(name, cmd));
                    ++j;
                }
            }

            return list;
        }

        public bool OnKeyPressed(KeyEventArg arg)
        {
            KeyCode code = arg.key;
            String cmd = FindCmd(code);
            if (cmd != null)
            {
                if (TryExecuteCommand(cmd))
                {
                    return true;
                }

                if (cmd[0] == '+')
                {
                    CVar var = FindVar(cmd.Substring(1));
                    if (var != null && var.intValue == 0)
                    {
                        var.SetValue(1);
                    }
                    return true;
                }
            }

            return false;
        }

        public bool OnKeyRepeated(KeyEventArg arg)
        {
            KeyCode code = arg.key;
            return FindCmd(code) != null;
        }

        public bool OnKeyReleased(KeyEventArg arg)
        {
            KeyCode code = arg.key;
            String cmd = FindCmd(code);
            if (cmd != null && cmd[0] == '+')
            {
                String cmdBase = cmd.Substring(1);
                String newCmd = '-' + cmdBase;
                if (TryExecuteCommand(newCmd))
                {
                    return true;
                }

                CVar var = FindVar(cmdBase);
                if (var != null && var.intValue == 1)
                {
                    var.SetValue(0);
                }
                return true;
            }

            return cmd != null;
        }

        private CVar FindVar(String name)
        {
            return Console.FindCvar(name);
        }

        private bool TryExecuteCommand(String cmd)
        {
            return Console.TryExecuteCommand(cmd);
        }

        private void ScheduleConfigUpdate()
        {
            Console.ScheduleConfigUpdate();
        }

        private CConsole Console
        {
            get { return Application.RootController().Console; }
        }
    }
}

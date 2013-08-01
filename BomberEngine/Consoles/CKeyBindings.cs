using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Input;
using BomberEngine.Util;
using BomberEngine.Debugging;

namespace BomberEngine.Consoles
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

    public class CKeyBindings
    {
        private String[] bindings;
        private int bindingsCount;
        
        public CKeyBindings()
        {
            bindings = new String[(int)KeyCode.TotalCount];
        }

        public void Bind(KeyCode code, String cmd)
        {
            int index = (int)code;
            if (bindings[index] == null)
            {
                ++bindingsCount;
            }

            bindings[index] = cmd;
        }

        public void Unbind(KeyCode code)
        {
            int index = (int)code;
            if (bindings[index] != null)
            {
                Debug.Assert(bindingsCount > 0);
                --bindingsCount;
            }
            bindings[index] = null;
        }

        public void UnbindAll()
        {
            ArrayUtils.Clear(bindings);
            bindingsCount = 0;
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
    }
}

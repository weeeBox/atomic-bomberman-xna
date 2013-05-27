using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Input
{
    public interface IKeyInputListener
    {
        bool OnKeyPressed(KeyEventArg arg);
        bool OnKeyRepeated(KeyEventArg arg);
        bool OnKeyReleased(KeyEventArg arg);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Input
{
    public interface InputListener : KeyboardListener, GamePadListener, GamePadStateListener, TouchListener
    {
    }
}

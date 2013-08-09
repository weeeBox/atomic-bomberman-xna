using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Events
{
    public class Notifications
    {
        public static readonly String GamePadConnected        = "GamePadConnected";         // playerIndex:int
        public static readonly String GamePadDisconnected     = "GamePadDisconnected";      // playerIndex:int

        public static readonly String ConsoleVisiblityChanged = "ConsoleVisiblityChanged";  // visible:bool
        public static readonly String ConsoleVariableChanged  = "ConsoleVariableChanged";   // cvar:Cvar
    }
}

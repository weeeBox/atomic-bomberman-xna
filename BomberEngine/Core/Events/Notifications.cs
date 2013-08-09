using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Events
{
    public class Notifications
    {
        public static readonly String GamePadConnected = "GamePadConnected";
        public static readonly String GamePadDisconnected = "GamePadDisconnected";

        public static readonly String ConsoleVisiblityChanged = "ConsoleVisiblityChanged";
        public static readonly String ConsoleVariableChanged  = "ConsoleVariableChanged";
    }
}

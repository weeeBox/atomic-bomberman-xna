using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Input
{
    public interface IGamePadStateListener
    {
        bool OnGamePadConnected(int playerIndex);
        bool OnGamePadDisconnected(int playerIndex);
    }
}

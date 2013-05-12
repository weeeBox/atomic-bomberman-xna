using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Input
{
    public interface IGamePadStateListener
    {
        void OnGamePadConnected(int playerIndex);
        void OnGamePadDisconnected(int playerIndex);
    }
}

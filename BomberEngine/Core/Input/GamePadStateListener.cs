using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Input
{
    public interface GamePadStateListener
    {
        void GamePadConnected(int playerIndex);
        void GamePadDisconnected(int playerIndex);
    }
}

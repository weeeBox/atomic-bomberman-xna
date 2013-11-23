using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Networking;

namespace BombermanTests.Mocks
{
    public class NetworkManagerMock : NetworkManager
    {
        public NetworkManagerMock()
        {
            networkPeer = new PeerMock();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Networking;
using Lidgren.Network;

namespace BombermanTests.Mocks
{
    public class PeerMock : Peer
    {
        public PeerMock()
            : base(null, 0)
        {
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        public override void SendMessage(NetOutgoingMessage message)
        {   
        }
    }
}

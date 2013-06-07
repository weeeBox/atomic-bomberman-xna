using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Network
{
    public class GameClientPeer : NetworkPeer
    {
        public GameClientPeer(String name, int port)
            : base(name, port)
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
    }
}

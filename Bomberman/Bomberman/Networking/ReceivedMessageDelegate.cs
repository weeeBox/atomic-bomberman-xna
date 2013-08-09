using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace Bomberman.Networking
{
    public delegate void ReceivedMessageDelegate(Peer peer, NetworkMessageId messageId, NetIncomingMessage message);
}

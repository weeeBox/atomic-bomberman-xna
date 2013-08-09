using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace Bomberman.Networking
{
    public delegate void ReceivedMessageDelegate<T>(T peer, NetworkMessageId messageId, NetIncomingMessage message);
}

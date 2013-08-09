using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Lidgren.Network;
using BomberEngine.Debugging;

namespace Bomberman.Networking
{
    public class ServerMessageReceivedDelegateList : BaseList<ServerMessageReceivedDelegate>
    {
        public ServerMessageReceivedDelegateList()
            : base(NullDelegate, 1)
        {
        }

        public override bool Add(ServerMessageReceivedDelegate e)
        {
            Debug.Assert(!Contains(e));
            return base.Add(e);
        }

        public void NotifyMessageReceived(Client client, NetworkMessageId messageId, NetIncomingMessage message)
        {
            int elementsCount = list.Count;
            for (int i = 0; i < elementsCount; ++i)
            {
                long readPos = message.Position;

                ServerMessageReceivedDelegate del = list[i];
                del(client, messageId, message);

                message.Position = readPos;
            }

            ClearRemoved();
        }

        private static void NullDelegate(Client client, NetworkMessageId messageId, NetIncomingMessage message)
        {
        }
    }
}

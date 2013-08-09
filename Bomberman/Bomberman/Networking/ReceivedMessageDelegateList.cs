using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Lidgren.Network;
using BomberEngine.Debugging;

namespace Bomberman.Networking
{
    public class ReceivedMessageDelegateList : BaseList<ReceivedMessageDelegate>
    {
        public ReceivedMessageDelegateList()
            : base(NullDelegate, 1)
        {
        }

        public override bool Add(ReceivedMessageDelegate e)
        {
            Debug.Assert(!Contains(e));
            return base.Add(e);
        }

        public void RemoveAll(Object target)
        {
            int elementsCount = list.Count;
            for (int i = 0; i < elementsCount; ++i)
            {
                ReceivedMessageDelegate del = list[i];
                if (del.Target == target)
                {
                    RemoveAt(i);
                }
            }
        }

        public void NotifyMessageReceived(Peer peer, NetworkMessageId messageId, NetIncomingMessage message)
        {
            int elementsCount = list.Count;
            for (int i = 0; i < elementsCount; ++i)
            {
                long readPos = message.Position;

                ReceivedMessageDelegate del = list[i];
                del(peer, messageId, message);

                message.Position = readPos;
            }

            ClearRemoved();
        }

        private static void NullDelegate(Peer peer, NetworkMessageId messageId, NetIncomingMessage message)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Networking;
using Lidgren.Network;
using System.Reflection;
using System.Globalization;

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
        }

        public override void Stop()
        {
        }

        public override void SendMessage(NetOutgoingMessage message)
        {
        }

        public override NetOutgoingMessage CreateMessage()
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            CultureInfo culture = null;
            return (NetOutgoingMessage)Activator.CreateInstance(typeof(NetOutgoingMessage), flags, null, new Type[0], culture);
        }

        public override NetOutgoingMessage CreateMessage(int initialCapacity)
        {
            return CreateMessage();
        }

        public override void RecycleMessage(NetOutgoingMessage msg)
        {   
        }

        public override void RecycleMessage(NetIncomingMessage msg)
        {
        }
    }
}

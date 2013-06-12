using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Bomberman.Network.Requests;
using Lidgren.Network;
using System.Net;
using BomberEngine.Core.IO;
using BomberEngine.Debugging;

namespace Bomberman.Network
{
    public abstract class NetworkPeer : IUpdatable
    {
        protected NetPeer peer;

        protected String name;
        protected int port;

        protected IDictionary<NetworkMessageID, NetworkMessage> commands;

        private BufferReader reader;
        private BufferWriter writer;

        protected NetworkPeer(String name, int port)
        {
            this.name = name;
            this.port = port;

            reader = new BufferReader();
            writer = new BufferWriter();

            commands = new Dictionary<NetworkMessageID, NetworkMessage>();
            RegisterMessages();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        public abstract void Start();
        public abstract void Stop();

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Updatable

        public void Update(float delta)
        {
            NetIncomingMessage msg;
            while ((msg = peer.ReadMessage()) != null)
            {
                HandleMessage(peer, msg);
            }
        }

        protected virtual bool HandleMessage(NetPeer peer, NetIncomingMessage msg)
        {
            switch (msg.MessageType)
            {
                case NetIncomingMessageType.StatusChanged:
                {
                    NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                    if (status == NetConnectionStatus.Connected)
                    {
                        OnPeerConnected(msg.SenderEndPoint);
                        return true;
                    }

                    if (status == NetConnectionStatus.Disconnected)
                    {
                        OnPeerDisconnected(msg.SenderEndPoint);
                        return true;
                    }

                    return false;
                }

                case NetIncomingMessageType.Data:
                {
                    reader.Init(msg.Data, msg.LengthBits);
                    NetworkMessage message = ReadMessage(reader);
                    reader.Reset();
                    break;
                }
            }

            return false;
        }

        protected virtual void OnPeerConnected(IPEndPoint endPoint)
        {
        }

        protected virtual void OnPeerDisconnected(IPEndPoint endPoint)
        {
        }

        protected virtual void OnMessageReceive(NetworkMessage message)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Messages

        private NetworkMessage ReadMessage(BufferReader reader)
        {
            NetworkMessageID id = (NetworkMessageID)reader.ReadByte();

            NetworkMessage message = FindMessage(id);
            Debug.Assert(message != null);

            message.Read(reader);
            return message;
        }

        private void RegisterMessages()
        {
            RegisterMessage(new MsgFieldState());
        }

        private void RegisterMessage(NetworkMessage command)
        {   
            commands.Add(command.id, command);
        }

        public NetworkMessage FindMessage(NetworkMessageID id)
        {
            NetworkMessage command;
            if (commands.TryGetValue(id, out command))
            {
                return command;
            }

            return null;
        }
            
        #endregion
    }
}

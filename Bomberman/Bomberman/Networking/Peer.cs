using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Lidgren.Network;
using System.Net;
using BomberEngine.Core.IO;
using BomberEngine.Debugging;
using BomberEngine.Game;

namespace Bomberman.Networking
{
    public interface IPeerListener
    {
        void OnPeerMessageReceived(Peer peer, NetIncomingMessage msg);
    }

    public abstract class Peer : BaseObject, IUpdatable
    {
        protected static readonly NetDeliveryMethod DefaultNetDeliveryMethod = NetDeliveryMethod.Unreliable;

        protected String m_name;
        protected int m_port;

        protected NetPeer m_peer;

        private static readonly IPeerListener s_nullPeerListener = new NullPeerListener();
        private IPeerListener m_listener;
        
        protected Peer(String name, int port)
        {
            m_name = name;
            m_port = port;
            SetPeerListener(null);
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
            while (m_peer != null && (msg = m_peer.ReadMessage()) != null)
            {
                HandleMessage(m_peer, msg);
            }
        }

        protected virtual bool HandleMessage(NetPeer peer, NetIncomingMessage msg)
        {
            Log.d(msg.MessageType != NetIncomingMessageType.Data, "Message received: " + msg.MessageType);

            switch (msg.MessageType)
            {
                case NetIncomingMessageType.StatusChanged:
                {
                    NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                    if (status == NetConnectionStatus.Connected)
                    {   
                        OnPeerConnected(msg.SenderConnection);
                        return true;
                    }

                    if (status == NetConnectionStatus.Disconnected)
                    {
                        OnPeerDisconnected(msg.SenderConnection);
                        return true;
                    }

                    return false;
                }

                case NetIncomingMessageType.Data:
                {
                    ReadMessage(msg);
                    break;
                }
            }

            return false;
        }

        protected virtual void OnPeerConnected(NetConnection connection)
        {
        }

        protected virtual void OnPeerDisconnected(NetConnection connection)
        {
        }

        protected virtual void OnMessageReceive(NetIncomingMessage message)
        {
            m_listener.OnPeerMessageReceived(this, message);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Messages

        private void ReadMessage(NetIncomingMessage msg)
        {
            OnMessageReceive(msg);
        }

        public void SendMessage(NetOutgoingMessage message, NetConnection recipient)
        {
            m_peer.SendMessage(message, recipient, DefaultNetDeliveryMethod);
        }

        public abstract void SendMessage(NetOutgoingMessage message);
        
        public NetOutgoingMessage CreateMessage()
        {
            return m_peer.CreateMessage();
        }

        public NetOutgoingMessage CreateMessage(int initialCapacity)
        {
            return m_peer.CreateMessage(initialCapacity);
        }

        public void RecycleMessage(NetOutgoingMessage msg)
        {
            m_peer.Recycle(msg);
        }

        public void RecycleMessage(NetIncomingMessage msg)
        {
            m_peer.Recycle(msg);
        }

        public void SetPeerListener(IPeerListener listener)
        {
            m_listener = listener != null ? listener : s_nullPeerListener;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public virtual NetConnection RemoteConnection
        {
            get { throw new NotImplementedException("Should be implemented in a subclass");  }
        }

        #endregion

        private class NullPeerListener : IPeerListener
        {
            public void OnPeerMessageReceived(Peer peer, NetIncomingMessage msg)
            {
            }
        }
    }
}

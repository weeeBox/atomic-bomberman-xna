using System;
using BomberEngine;
using Lidgren.Network;
using System.Collections.Generic;

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

            RecordNetworkTick();
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
                        ReadPeerConnected(msg);
                        return true;
                    }

                    if (status == NetConnectionStatus.Disconnected)
                    {
                        ReadPeerDisconnected(msg);
                        return true;
                    }

                    return false;
                }

                case NetIncomingMessageType.Data:
                {
                    ReadMessage(msg);
                    return true;
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

        private void ReadPeerConnected(NetIncomingMessage msg)
        {
            RecordPeerConnected(msg);
            OnPeerConnected(msg.SenderConnection);
        }

        private void ReadPeerDisconnected(NetIncomingMessage msg)
        {
            RecordPeerDisconnected(msg);
            OnPeerDisconnected(msg.SenderConnection);
        }

        private void ReadMessage(NetIncomingMessage msg)
        {
            RecordMessage(msg);
            OnMessageReceive(msg);
        }

        public void SendMessage(NetOutgoingMessage message, NetConnection recipient)
        {
            m_peer.SendMessage(message, recipient, DefaultNetDeliveryMethod);
        }

        public abstract void SendMessage(NetOutgoingMessage message);
        
        public virtual NetOutgoingMessage CreateMessage()
        {
            return m_peer.CreateMessage();
        }

        public virtual NetOutgoingMessage CreateMessage(int initialCapacity)
        {
            return m_peer.CreateMessage(initialCapacity);
        }

        public virtual void RecycleMessage(NetOutgoingMessage msg)
        {
            m_peer.Recycle(msg);
        }

        public virtual void RecycleMessage(NetIncomingMessage msg)
        {
            m_peer.Recycle(msg);
        }

        public void SetPeerListener(IPeerListener listener)
        {
            m_listener = listener != null ? listener : s_nullPeerListener;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Demo

        private IDictionary<NetConnection, int> m_connectionIndexLookup = new Dictionary<NetConnection, int>();
        private int m_nextConnectionIndex = -1;

        [System.Diagnostics.Conditional("DEBUG_DEMO")]
        private void RecordNetworkTick()
        {
            DemoRecorder.Instance.WriteNetworkTick();
        }

        [System.Diagnostics.Conditional("DEBUG_DEMO")]
        private void RecordPeerConnected(NetIncomingMessage msg)
        {   
            Assert.AreEqual(-1, ConnectionIndex(msg.SenderConnection));

            m_connectionIndexLookup[msg.SenderConnection] = ++m_nextConnectionIndex;
            DemoRecorder.Instance.WritePeerConnected(m_nextConnectionIndex);
        }

        [System.Diagnostics.Conditional("DEBUG_DEMO")]
        private void RecordPeerDisconnected(NetIncomingMessage msg)
        {
            int index = ConnectionIndex(msg.SenderConnection);
            Assert.IsTrue(index != -1);

            m_connectionIndexLookup.Remove(msg.SenderConnection);
            DemoRecorder.Instance.WritePeerDisconnected(index);
        }

        [System.Diagnostics.Conditional("DEBUG_DEMO")]
        private void RecordMessage(NetIncomingMessage msg)
        {
            DemoRecorder.Instance.WritePeerMessage(msg.LengthBits, msg.Data);
        }

        private int ConnectionIndex(NetConnection con)
        {   
            int index;
            if (m_connectionIndexLookup.TryGetValue(con, out index))
            {
                return index;
            }
            return -1;
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

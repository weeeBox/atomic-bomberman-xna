using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace Bomberman.Networking
{
    public class ReceivedMessageDelegateRegistry
    {
        private IDictionary<NetworkMessageId, ReceivedMessageDelegateList> m_map;

        public ReceivedMessageDelegateRegistry()
        {
            m_map = new Dictionary<NetworkMessageId, ReceivedMessageDelegateList>();
        }

        public void Add(NetworkMessageId messageId, ReceivedMessageDelegate del)
        {
            ReceivedMessageDelegateList list = FindList(messageId);
            if (list == null)
            {
                list = new ReceivedMessageDelegateList();
                m_map[messageId] = list;
            }
            list.Add(del);
        }

        public void Remove(NetworkMessageId messageId, ReceivedMessageDelegate del)
        {
            ReceivedMessageDelegateList list = FindList(messageId);
            if (list != null)
            {
                list.Remove(del);
            }
        }

        public void Remove(ReceivedMessageDelegate del)
        {
            foreach (KeyValuePair<NetworkMessageId, ReceivedMessageDelegateList> e in m_map)
            {
                ReceivedMessageDelegateList list = e.Value;
                list.Remove(del);
            }
        }

        public void RemoveAll(Object target)
        {
            foreach (KeyValuePair<NetworkMessageId, ReceivedMessageDelegateList> e in m_map)
            {
                ReceivedMessageDelegateList list = e.Value;
                list.RemoveAll(target);
            }
        }

        public void NotifyMessageReceived(Peer peer, NetworkMessageId messageId, NetIncomingMessage message)
        {
            ReceivedMessageDelegateList list = FindList(messageId);
            if (list != null)
            {
                list.NotifyMessageReceived(peer, messageId, message);
            }
        }

        private ReceivedMessageDelegateList FindList(NetworkMessageId messageId)
        {
            ReceivedMessageDelegateList list;
            if (m_map.TryGetValue(messageId, out list))
            {
                return list;
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace Bomberman.Networking
{
    public class ReceivedMessageDelegateRegistry<T>
    {
        private IDictionary<NetworkMessageId, ReceivedMessageDelegateList<T>> m_map;

        public ReceivedMessageDelegateRegistry()
        {
            m_map = new Dictionary<NetworkMessageId, ReceivedMessageDelegateList<T>>();
        }

        public void Add(NetworkMessageId messageId, ReceivedMessageDelegate<T> del)
        {
            ReceivedMessageDelegateList<T> list = FindList(messageId);
            if (list == null)
            {
                list = new ReceivedMessageDelegateList<T>();
                m_map[messageId] = list;
            }
            list.Add(del);
        }

        public void Remove(NetworkMessageId messageId, ReceivedMessageDelegate<T> del)
        {
            ReceivedMessageDelegateList<T> list = FindList(messageId);
            if (list != null)
            {
                list.Remove(del);
            }
        }

        public void Remove(ReceivedMessageDelegate<T> del)
        {
            foreach (KeyValuePair<NetworkMessageId, ReceivedMessageDelegateList<T>> e in m_map)
            {
                ReceivedMessageDelegateList<T> list = e.Value;
                list.Remove(del);
            }
        }

        public void RemoveAll(Object target)
        {
            foreach (KeyValuePair<NetworkMessageId, ReceivedMessageDelegateList<T>> e in m_map)
            {
                ReceivedMessageDelegateList<T> list = e.Value;
                list.RemoveAll(target);
            }
        }

        public void NotifyMessageReceived(T peer, NetworkMessageId messageId, NetIncomingMessage message)
        {
            ReceivedMessageDelegateList<T> list = FindList(messageId);
            if (list != null)
            {
                list.NotifyMessageReceived(peer, messageId, message);
            }
        }

        private ReceivedMessageDelegateList<T> FindList(NetworkMessageId messageId)
        {
            ReceivedMessageDelegateList<T> list;
            if (m_map.TryGetValue(messageId, out list))
            {
                return list;
            }
            return null;
        }
    }
}

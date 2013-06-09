using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace Bomberman.Network.Requests
{
    public abstract class PeerData
    {
        private byte id;

        protected PeerData(byte id)
        {
            this.id = id;
        }   

        public void Write(NetBuffer stream)
        {
            stream.Write(id);
            WriteData(stream);
        }

        public void Read(NetBuffer stream)
        {
            ReadData(stream);
        }

        protected abstract void WriteData(NetBuffer stream);
        protected abstract void ReadData(NetBuffer stream);
    }
}

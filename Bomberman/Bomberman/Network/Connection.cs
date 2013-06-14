using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;

namespace Bomberman.Network
{
    public class Connection
    {
        public NetConnection connection;
        public String name;
        public int index;

        public Connection(String name, NetConnection connection)
        {
            this.name = name;
            this.connection = connection;
        }
    }
}

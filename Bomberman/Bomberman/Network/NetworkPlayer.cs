using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Bomberman.Network
{
    public class NetworkPlayer
    {
        public IPEndPoint endPoint;
        public String name;
        public int index;

        public NetworkPlayer(String name, IPEndPoint endPoint)
        {
            this.name = name;
            this.endPoint = endPoint;
        }
    }
}

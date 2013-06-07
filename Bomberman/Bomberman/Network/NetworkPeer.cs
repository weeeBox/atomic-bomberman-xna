using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;

namespace Bomberman.Network
{
    public abstract class NetworkPeer : IUpdatable
    {
        public abstract void Start();
        public abstract void Stop();

        public virtual void Update(float delta)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Bomberman.Network.Requests;

namespace Bomberman.Network
{
    public abstract class NetworkPeer : IUpdatable
    {
        protected String name;
        protected int port;

        protected IDictionary<byte, PeerCommand> commands;

        protected NetworkPeer(String name, int port)
        {
            this.name = name;
            this.port = port;

            commands = new Dictionary<byte, PeerCommand>();
            RegisterCommands();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        public abstract void Start();
        public abstract void Stop();

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Updatable

        public virtual void Update(float delta)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Commands

        private void RegisterCommands()
        {
            RegisterCommand(new MapPeerCommand());
        }

        private void RegisterCommand(PeerCommand command)
        {
            byte id = command.id;
            commands.Add(id, command);
        }

        public PeerCommand FindCommand(byte id)
        {
            PeerCommand command;
            if (commands.TryGetValue(id, out command))
            {
                return command;
            }

            return null;
        }
            
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using Bomberman.Network;
using BomberEngine.Debugging;
using Bomberman.Game.Screens;
using BomberEngine.Core;

namespace Bomberman.Game
{
    public class MultiplayerController : Controller
    {
        private LocalServersDiscovery serverDiscovery;
        private List<ServerInfo> foundServers;

        private GameLobbyScreen lobbyScreen;

        protected override void OnStart()
        {
            lobbyScreen = new GameLobbyScreen(false);
            StartScreen(lobbyScreen);

            StartDiscovery();
        }

        protected override void OnStop()
        {
            StopDiscovery(false);
        }

        #region Local server discovery

        private void StartDiscovery()
        {
            Debug.Assert(serverDiscovery == null);

            String name = CVars.sv_name.value;
            int port = CVars.sv_port.intValue;

            serverDiscovery = new LocalServersDiscovery(OnLocalServerFound, name, port);
            serverDiscovery.Start();

            foundServers = new List<ServerInfo>();

            lobbyScreen.AddUpdatable(UpdateDiscovery);
            lobbyScreen.ScheduleCall(StopDiscoveryCall, 5.0f);

            Log.i("Started local servers discovery...");
            lobbyScreen.SetBusy();
        }

        private void UpdateDiscovery(float delta)
        {
            serverDiscovery.Update(delta);
        }

        private void StopDiscoveryCall(DelayedCall call)
        {
            StopDiscovery(true);
        }

        private void StopDiscovery(bool updateUI)
        {
            if (serverDiscovery != null)
            {
                serverDiscovery.Stop();
                serverDiscovery = null;

                lobbyScreen.RemoveUpdatable(UpdateDiscovery);

                if (updateUI)
                {
                    lobbyScreen.SetServers(foundServers);
                }

                Log.i("Stopped local servers discovery");
            }
        }

        private void OnLocalServerFound(ServerInfo info)
        {
            Log.d("Found local server: " + info.endPoint);
            foundServers.Add(info);
        }

        #endregion
    }
}

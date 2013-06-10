using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using Bomberman.Menu.Screens;
using BomberEngine.Core.Visual;
using Bomberman.Game;
using Bomberman.Network;
using BomberEngine.Debugging;

namespace Bomberman.Menu
{
    public class MenuController : Controller
    {
        public enum ExitCode
        {
            Quit,
            SingleStart,
            MultiplayerStart,
            MultiplayerJoin
        }

        public LocalServersDiscovery serverDiscovery;

        private void Stop(ExitCode code)
        {
            Stop((int)code);
        }

        protected override void OnStart()
        {
            StartScreen(new MainMenuScreen(OnMainMenuButtonPress));
        }

        protected override void OnStop()
        {
            StopDiscovery();
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (serverDiscovery != null)
            {
                serverDiscovery.Update(delta);
            }
        }

        private void OnMainMenuButtonPress(Button button)
        {   
            MainMenuScreen.ButtonId buttonId = (MainMenuScreen.ButtonId)button.id;
            switch (buttonId)
            {
                case MainMenuScreen.ButtonId.Play:
                    Stop(ExitCode.SingleStart);
                    break;
                case MainMenuScreen.ButtonId.Multiplayer:
                    Screen multiplayerScreen = new MultiplayerScreen(OnMultiplayerButtonPress);
                    multiplayerScreen.onStartDelegate = OnMultiplayerScreenStart;
                    multiplayerScreen.onStopDelegate = OnMultiplayerScreenStop;
                    StartNextScreen(multiplayerScreen);
                    break;
                case MainMenuScreen.ButtonId.Settings:
                    StartNextScreen(new SettingsScreen(OnSettingsButtonPress));
                    break;
                case MainMenuScreen.ButtonId.Exit:
                    Stop(ExitCode.Quit);
                    break;
            }
        }

        private void OnMultiplayerButtonPress(Button button)
        {
            MultiplayerScreen.ButtonId buttonId = (MultiplayerScreen.ButtonId)button.id;
            switch (buttonId)
            {
                case MultiplayerScreen.ButtonId.Create:
                    Stop(ExitCode.MultiplayerStart);
                    break;
                case MultiplayerScreen.ButtonId.Join:
                    Stop(ExitCode.MultiplayerJoin);
                    break;
            }
        }

        private void OnSettingsButtonPress(Button button)
        {
        }

        private void OnMultiplayerScreenStart(Screen Screen)
        {
            StartDiscovery();
        }

        private void OnMultiplayerScreenStop(Screen Screen)
        {
            StopDiscovery();
        }

        private void StartDiscovery()
        {
            Debug.Assert(serverDiscovery == null);

            String name = CVars.sv_name.value;
            int port = CVars.sv_port.intValue;

            serverDiscovery = new LocalServersDiscovery(OnLocalServerFound, name, port);
            serverDiscovery.Start();

            Log.i("Started local servers discovery...");
        }

        private void StopDiscovery()
        {
            if (serverDiscovery != null)
            {
                serverDiscovery.Stop();
                serverDiscovery = null;

                Log.i("Stopped local servers discovery");
            }
        }

        private void OnLocalServerFound(ServerInfo info)
        {
            Log.d("Found local server: " + info.endPoint);
        }
    }
}

using System;
using BomberEngine;
using Bomberman.Game.Screens;
using Bomberman.Menu.Screens;

namespace Bomberman.Menu
{
    public class MenuController : Controller
    {
        public enum ExitCode
        {
            Quit,
            SingleStart,
            MultiplayerStart,
            DebugServerStart,
            DebugClientStart,
        }

        public enum ScreenID
        {
            MainMenu,
            Multiplayer,
            Settings,
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        private void Stop(ExitCode code, Object data = null)
        {
            Stop((int)code, data);
        }

        protected override void OnStart()
        {
            Application.SetWindowTitle("");
            StartScreen(new MainMenuScreen(MainMenuScreenButtonDelegate));
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Button delegates

        private void MainMenuScreenButtonDelegate(Button button)
        {   
            MainMenuScreen.ButtonId buttonId = (MainMenuScreen.ButtonId)button.id;
            switch (buttonId)
            {
                case MainMenuScreen.ButtonId.Play:
                    Stop(ExitCode.SingleStart);
                    break;
                case MainMenuScreen.ButtonId.Multiplayer:
                    Stop(ExitCode.MultiplayerStart);
                    break;
                case MainMenuScreen.ButtonId.DebugStartServer:
                    Stop(ExitCode.DebugServerStart);
                    break;
                case MainMenuScreen.ButtonId.DebugStartClient:
                    Stop(ExitCode.DebugClientStart);
                    break;
                case MainMenuScreen.ButtonId.Settings:
                    StartNextScreen(new SettingsScreen(OnSettingsButtonPress));
                    break;
                case MainMenuScreen.ButtonId.Exit:
                    Stop(ExitCode.Quit);
                    break;
                case MainMenuScreen.ButtonId.Test:
                    TestSomething();
                    break;
            }
        }

        private void OnSettingsButtonPress(Button button)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Testing

        private void TestSomething()
        {
            StartNextScreen(new TestScreen());
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        private Screen FindScreen(ScreenID id)
        {
            return FindScreen((int)id);
        }

        #endregion
    }
}

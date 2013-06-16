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
            StartScreen(new MainMenuScreen(OnMainMenuButtonPress));
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Button delegates

        private void OnMainMenuButtonPress(Button button)
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
                case MainMenuScreen.ButtonId.Settings:
                    StartNextScreen(new SettingsScreen(OnSettingsButtonPress));
                    break;
                case MainMenuScreen.ButtonId.Exit:
                    Stop(ExitCode.Quit);
                    break;
            }
        }

        private void OnSettingsButtonPress(Button button)
        {
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

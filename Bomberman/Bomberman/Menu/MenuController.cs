using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using Bomberman.Menu.Screens;
using BomberEngine.Core.Visual;
using Bomberman.Game;

namespace Bomberman.Menu
{
    public class MenuController : Controller
    {
        protected override void OnStart()
        {
            StartScreen(new MainMenuScreen(OnMainMenuButtonPress));
        }

        private void OnMainMenuButtonPress(Button button)
        {   
            MainMenuScreen.ButtonId buttonId = (MainMenuScreen.ButtonId)button.id;
            switch (buttonId)
            {
                case MainMenuScreen.ButtonId.Play:
                    Stop(ExitCode.StartGame);
                    break;
                case MainMenuScreen.ButtonId.Multiplayer:
                    StartNextScreen(new MultiplayerScreen(OnMultiplayerButtonPress));
                    break;
                case MainMenuScreen.ButtonId.Settings:
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
                    break;
                case MultiplayerScreen.ButtonId.Join:
                    break;
            }
        }
    }
}

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
    public class MenuController : Controller, IButtonDelegate
    {
        protected override void OnStart()
        {
            StartScreen(new MainMenuScreen(this));
        }

        public void OnButtonPress(AbstractButton button)
        {
            MainMenuScreen.ButtonId buttonId = (MainMenuScreen.ButtonId)button.id;
            switch (buttonId)
            {
                case MainMenuScreen.ButtonId.Play:
                    Stop(ExitCode.StartGame);
                    break;
                case MainMenuScreen.ButtonId.Settings:
                    break;
                case MainMenuScreen.ButtonId.Exit:
                    Stop(ExitCode.Quit);
                    break;
            }
        }
    }
}

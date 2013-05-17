using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual.UI;
using Microsoft.Xna.Framework;
using BomberEngine.Core.Visual;
using Bomberman.Game;
using Assets;
using BomberEngine.Core.Assets.Types;

namespace Bomberman.Menu.Screens
{
    public class MainMenuScreen : Screen
    {
        public enum ButtonId
        {
            Play,
            Settings,
            Exit
        }

        public MainMenuScreen(IButtonDelegate buttonDelegate)
        {
            int w = 150;
            int h = 20;

            Font font = Helper.GetFont(A.fnt_button);

            View rootView = new View();
            rootView.alignX = rootView.alignY = ALIGN_CENTER;

            Button button = new Button("Play", font, 0, 0, w, h);
            button.id = (int)ButtonId.Play;
            button.SetDelegate(buttonDelegate);
            rootView.AddView(button);

            button = new Button("Settings", font, 0, 0, w, h);
            button.id = (int)ButtonId.Settings;
            button.SetDelegate(buttonDelegate);
            rootView.AddView(button);

            button = new Button("Exit", font, 0, 0, w, h);
            button.id = (int)ButtonId.Exit;
            button.SetDelegate(buttonDelegate);
            rootView.AddView(button);

            rootView.LayoutVer(20);
            rootView.ResizeToFitViewsVer();

            SetRootView(rootView);

            rootView.x = 0.5f * width;
            rootView.y = 0.5f * height;
        }
    }
}

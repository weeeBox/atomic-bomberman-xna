using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Assets.Types;
using Bomberman.Game;
using Assets;
using BomberEngine.Core.Visual;

namespace Bomberman.Menu.Screens
{
    public class SettingsScreen : Screen
    {
        public enum ButtonId
        {
            Back
        }

        public SettingsScreen(ButtonDelegate buttonDelegate)
            : base((int)MenuController.ScreenID.Settings)
        {
            int w = 150;
            int h = 20;

            Font font = Helper.GetFont(A.fnt_button);

            View rootView = new View();
            rootView.alignX = rootView.alignY = View.ALIGN_CENTER;

            TextButton button = new TextButton("Back", font, 0, 0, w, h);
            button.id = (int)ButtonId.Back;
            button.SetDelegate(OnButtonPressed);
            rootView.AddView(button);

            rootView.LayoutVer(20);
            rootView.ResizeToFitViewsVer();

            AddView(rootView);

            rootView.x = 0.5f * width;
            rootView.y = 0.5f * height;
        }

        private void OnButtonPressed(Button button)
        {
            ButtonId buttonId = (ButtonId)button.id;
            switch (buttonId)
            {
                case ButtonId.Back:
                    Finish();
                    break;
            }
        }
    }
}

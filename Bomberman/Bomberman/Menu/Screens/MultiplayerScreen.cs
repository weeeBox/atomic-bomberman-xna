using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Assets.Types;
using Bomberman.Game;
using Assets;

namespace Bomberman.Menu.Screens
{
    public class MultiplayerScreen : Screen
    {
        public enum ButtonId
        {
            Create,
            Join,
            Back
        }

        public MultiplayerScreen(ButtonDelegate buttonDelegate)
        {
            int w = 150;
            int h = 20;

            Font font = Helper.GetFont(A.fnt_button);

            View rootView = new View();
            rootView.alignX = rootView.alignY = View.ALIGN_CENTER;

            TextButton button = new TextButton("Create", font, 0, 0, w, h);
            button.id = (int)ButtonId.Create;
            button.SetDelegate(buttonDelegate);
            rootView.AddView(button);

            button = new TextButton("Join", font, 0, 0, w, h);
            button.id = (int)ButtonId.Join;
            button.SetDelegate(buttonDelegate);
            rootView.AddView(button);

            button = new TextButton("Back", font, 0, 0, w, h);
            button.id = (int)ButtonId.Back;
            button.SetDelegate(OnButtonPress);
            rootView.AddView(button);

            rootView.LayoutVer(20);
            rootView.ResizeToFitViewsVer();

            AddView(rootView);

            rootView.alignX = rootView.alignY = View.ALIGN_CENTER;

            rootView.x = 0.5f * width;
            rootView.y = 0.5f * height;
        }

        private void OnButtonPress(Button button)
        {
            ButtonId buttonId = (ButtonId)button.id;
            if (buttonId == ButtonId.Back)
            {
                Finish();
            }
        }
    }
}

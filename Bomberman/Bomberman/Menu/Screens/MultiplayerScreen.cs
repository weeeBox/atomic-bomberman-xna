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
            Refresh,
            Back
        }

        public MultiplayerScreen(ButtonDelegate buttonDelegate)
        {
            int w = 100;
            int h = 20;

            Font font = Helper.GetFont(A.fnt_button);

            View rootView = new View();

            TextButton button = new TextButton("Back", font, 0, 0, w, h);
            button.id = (int)ButtonId.Back;
            button.SetDelegate(OnButtonPress);
            rootView.AddView(button);

            button = new TextButton("Refresh", font, 0, 0, w, h);
            button.id = (int)ButtonId.Refresh;
            button.SetDelegate(buttonDelegate);
            rootView.AddView(button);

            button = new TextButton("Create", font, 0, 0, w, h);
            button.id = (int)ButtonId.Create;
            button.SetDelegate(buttonDelegate);
            rootView.AddView(button);

            button = new TextButton("Join", font, 0, 0, w, h);
            button.id = (int)ButtonId.Join;
            button.SetDelegate(buttonDelegate);
            rootView.AddView(button);

            rootView.LayoutHor(0);
            rootView.ResizeToFitViews(true, true, 20);

            AddView(rootView);

            rootView.x = 0.5f * (width - rootView.width);
            rootView.y = height - rootView.height;
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

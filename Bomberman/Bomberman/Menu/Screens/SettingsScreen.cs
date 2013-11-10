using BomberEngine;
using Bomberman.Gameplay;
using Bomberman.UI;

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
            Font font = Helper.fontButton;

            View rootView = new View();
            rootView.alignX = rootView.alignY = View.ALIGN_CENTER;

            TextButton button = new TempButton("Back");
            button.id = (int)ButtonId.Back;
            button.buttonDelegate = OnButtonPressed;
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

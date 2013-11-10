using BomberEngine;
using Bomberman.Gameplay;
using Bomberman.UI;

namespace Bomberman.Menu.Screens
{
    public class MainMenuScreen : Screen
    {
        public enum ButtonId
        {
            Play,
            Multiplayer,
            DebugStartServer,
            DebugStartClient,
            Settings,
            Test,
            Exit
        }

        public MainMenuScreen(ButtonDelegate buttonDelegate)
            : base((int)MenuController.ScreenID.MainMenu)
        {
            IsFinishOnCancel = false;

            Font font = Helper.fontButton;

            View rootView = new View();

            TextButton button = new TempButton("Play");
            button.id = (int)ButtonId.Play;
            button.buttonDelegate = buttonDelegate;
            rootView.AddView(button);

            button = new TempButton("Test");
            button.id = (int)ButtonId.Test;
            button.buttonDelegate = buttonDelegate;
            rootView.AddView(button);

            button = new TempButton("Join server");
            button.id = (int)ButtonId.DebugStartClient;
            button.buttonDelegate = buttonDelegate;
            rootView.AddView(button);

            button = new TempButton("Start server");
            button.id = (int)ButtonId.DebugStartServer;
            button.buttonDelegate = buttonDelegate;
            rootView.AddView(button);

            //button = new TempButton("Multiplayer", font, 0, 0, w, h);
            //button.id = (int)ButtonId.Multiplayer;
            //button.SetDelegate(buttonDelegate);
            //rootView.AddView(button);

            button = new TempButton("Settings");
            button.id = (int)ButtonId.Settings;
            button.buttonDelegate = buttonDelegate;
            rootView.AddView(button);

            button = new TempButton("Exit");
            button.id = (int)ButtonId.Exit;
            button.buttonDelegate = buttonDelegate;
            rootView.AddView(button);

            rootView.LayoutVer(20);
            rootView.ResizeToFitViews();

            AddView(rootView);

            rootView.x = 0.5f * (width - rootView.width);
            rootView.y = 0.5f * (height - rootView.height);
        }
    }
}

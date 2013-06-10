using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Assets.Types;
using Bomberman.Game;
using Assets;
using Bomberman.Network;
using BomberEngine.Debugging;
using BomberEngine.Core;

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

        private LocalServersDiscovery serverDiscovery;

        public MultiplayerScreen(ButtonDelegate buttonDelegate)
            : base((int)MenuController.ScreenID.Multiplayer)
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

        protected override void OnStart()
        {
            StartDiscovery();
        }

        protected override void OnStop()
        {
            StopDiscovery();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Local servers discovery

        private void StartDiscovery()
        {
            Debug.Assert(serverDiscovery == null);

            String name = CVars.sv_name.value;
            int port = CVars.sv_port.intValue;

            serverDiscovery = new LocalServersDiscovery(OnLocalServerFound, name, port);
            serverDiscovery.Start();

            AddUpdatable(UpdateDiscovery);

            Log.i("Started local servers discovery...");
        }

        private void UpdateDiscovery(float delta)
        {
            serverDiscovery.Update(delta);
        }

        private void StopDiscovery()
        {
            if (serverDiscovery != null)
            {
                serverDiscovery.Stop();
                serverDiscovery = null;

                RemoveUpdatable(UpdateDiscovery);

                Log.i("Stopped local servers discovery");
            }
        }

        private void OnLocalServerFound(ServerInfo info)
        {
            Log.d("Found local server: " + info.endPoint);

        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

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

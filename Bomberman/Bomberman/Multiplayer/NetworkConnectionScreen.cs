using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using Assets;
using BomberEngine.Core.Assets.Types;
using Bomberman.Game;

namespace Bomberman.Multiplayer
{
    public delegate void ConnectionCancelCallback();

    public class NetworkConnectionScreen : Screen
    {
        public enum ButtonId
        {
            Cancel
        }

        public ConnectionCancelCallback cancelCallback;

        private TextView statusTextView;

        public NetworkConnectionScreen(String message = "Connecting...")
        {
            Font font = Helper.fontButton;

            statusTextView = new TextView(font, message);
            statusTextView.alignX = statusTextView.alignY = View.ALIGN_CENTER;
            statusTextView.x = 0.5f * width;
            statusTextView.y = 0.5f * height;

            AddView(statusTextView);

            Button cancelButton = new TextButton("Cancel", font, 0, 0, 100, 20);
            cancelButton.buttonDelegate = OnButtonPress;
            cancelButton.id = (int)ButtonId.Cancel;

            cancelButton.alignX = View.ALIGN_CENTER;
            cancelButton.x = 0.5f * width;
            cancelButton.y = statusTextView.y + 1.5f * statusTextView.height + 10;
            SetBackButton(cancelButton);

            AddView(cancelButton);
        }

        public void SetStatusText(String text)
        {
            statusTextView.SetText(text);
        }

        private void OnButtonPress(Button button)
        {
            ButtonId buttonId = (ButtonId)button.id;
            switch (buttonId)
            {
                case ButtonId.Cancel:
                {
                    cancelCallback();
                    break;
                }
            }
        }
    }
}

using System;
using BomberEngine;
using Bomberman.Game;
using Bomberman.UI;

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

            Button cancelButton = new TempButton("Cancel");
            cancelButton.buttonDelegate = OnButtonPress;
            cancelButton.id = (int)ButtonId.Cancel;

            cancelButton.alignX = View.ALIGN_CENTER;
            cancelButton.x = 0.5f * width;
            cancelButton.y = statusTextView.y + 1.5f * statusTextView.height + 10;
            SetCancelButton(cancelButton);

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

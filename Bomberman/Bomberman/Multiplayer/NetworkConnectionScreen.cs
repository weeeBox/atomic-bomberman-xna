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
    public delegate void ConnectionCancelCallback(int operationId);

    public class NetworkConnectionScreen : Screen
    {
        public enum ButtonId
        {
            Cancel
        }

        public int operationId;
        private ConnectionCancelCallback cancelCallback;

        public NetworkConnectionScreen(ConnectionCancelCallback cancelCallback)
        {
            this.cancelCallback = cancelCallback;

            Font font = Helper.GetFont(A.fnt_button);

            TextView textView = new TextView(font, "Connecting to the server...");
            textView.x = 0.5f * (width - textView.width);
            textView.y = 0.5f * (height - textView.height);

            AddView(textView);

            Button cancelButton = new TextButton("Cancel", 0, 0, 100, 20);
            cancelButton.SetDelegate(OnButtonPress);
            cancelButton.id = (int)ButtonId.Cancel;

            cancelButton.alignX = View.ALIGN_CENTER;
            cancelButton.x = 0.5f * width;
            cancelButton.y = textView.y + textView.height + 10;

            AddView(cancelButton);
        }

        private void OnButtonPress(Button button)
        {
            ButtonId buttonId = (ButtonId)button.id;
            switch (buttonId)
            {
                case ButtonId.Cancel:
                {
                    cancelCallback(operationId);
                    break;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual.UI;
using Microsoft.Xna.Framework;
using BomberEngine.Core.Visual;

namespace Bomberman.Common.Popups
{
    public delegate void DialogPopupDelegate(DialogPopup popup, int buttonId);

    public struct PopupButton
    {
        public int id;
        public String title;
        
        public PopupButton(int id, String title)
        {
            this.id = id;
            this.title = title;
        }
    }

    public class DialogPopup : Popup
    {
        public enum ButtonId
        {
            Ok,
            Cancel,
            Abort,
            Yes,
            No,
        }

        public enum MessageType
        {
            None,
            Info,
            Warning,
            Error
        }

        public DialogPopup(DialogPopupDelegate popupDelegate, String title, String message, params PopupButton[] buttons)
        {
            RectView background = new RectView(0, 0, width, height, new Color(0, 0, 0, 0.25f), Color.Transparent);
            AddView(background);

            RectView content = new RectView(0, 0, 0, 0, Color.Black, Color.Transparent);

            content.ResizeToFitViews(true, true, 50);
            AddView(content);
        }
    }
}

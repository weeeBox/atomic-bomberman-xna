using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual.UI;
using Microsoft.Xna.Framework;
using BomberEngine.Core.Visual;
using BomberEngine.Util;
using BomberEngine.Core.Assets.Types;
using Bomberman.Game;
using Assets;
using Bomberman.Menu;

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
        public enum MessageType
        {
            None,
            Info,
            Warning,
            Error
        }

        public enum ButtonId
        {
            Ok,
            Cancel,
            Abort,
            Yes,
            No,
        }

        public static readonly PopupButton ButtonOk     = new PopupButton((int)ButtonId.Ok, "OK");
        public static readonly PopupButton ButtonCancel = new PopupButton((int)ButtonId.Cancel, "CANCEL");
        public static readonly PopupButton ButtonAbort  = new PopupButton((int)ButtonId.Abort, "ABORT");
        public static readonly PopupButton ButtonYes    = new PopupButton((int)ButtonId.Yes, "YES");
        public static readonly PopupButton ButtonNo     = new PopupButton((int)ButtonId.No, "NO");

        private DialogPopupDelegate popupDelegate;

        public DialogPopup(DialogPopupDelegate popupDelegate, String title, String message, PopupButton cancelButton, params PopupButton[] buttons)
        {
            this.popupDelegate = popupDelegate;

            AddView(new RectView(0, 0, width, height, new Color(0.0f, 0.0f, 0.0f, 0.25f), Color.Black));

            RectView frameView = new RectView(0, 0, 0.75f * width, 0, new Color(0.0f, 0.0f, 0.0f, 0.75f), Color.Black);

            View content = new View();
            content.width = frameView.width - 2 * 50;
            content.alignX = content.alignY = View.ALIGN_CENTER;

            Font font = Helper.GetFont(A.fnt_system);

            // title
            TextView titleView = new TextView(font, title);
            titleView.alignX = titleView.parentAlignX = View.ALIGN_CENTER;
            content.AddView(titleView);

            // message
            TextView messageView = new TextView(font, message, (int)content.width);
            messageView.alignX = messageView.parentAlignX = View.ALIGN_CENTER;
            content.AddView(messageView);

            // buttons
            Button button = new TextButton(cancelButton.title, 0, 0, 100, 20);
            button.id = (int)cancelButton.id;
            button.SetDelegate(OnButtonPress);
            button.alignX = View.ALIGN_CENTER;
            button.parentAlignX = View.ALIGN_CENTER;
            SetBackButton(button);

            content.AddView(button);

            content.LayoutVer(10);
            content.ResizeToFitViewsVer();

            frameView.AddView(content);

            frameView.ResizeToFitViewsVer(20, 20);
            frameView.alignX = frameView.alignY = frameView.parentAlignX = frameView.parentAlignY = View.ALIGN_CENTER;
            AddView(frameView);

            content.x = 0.5f * frameView.width;
            content.y = 0.5f * frameView.height;
        }

        public override void Draw(Context context)
        {
            base.Draw(context);
        }

        private void OnButtonPress(Button button)
        {
            NotifyPopupDelegate(button.id);
        }

        public static void ShowMessage(String format, params Object[] args)
        {
            ShowMessage(null, format, args);
        }

        public static void ShowMessage(DialogPopupDelegate popupDelegate, String format, params Object[] args)
        {
            String message = StringUtils.TryFormat(format, args);
            DialogPopup popup = new DialogPopup(popupDelegate, "Message", message, ButtonOk);
            Application.RootController().CurrentController.StartPopup(popup);
        }

        private static void DefaultDialogPopupDelegate(DialogPopup popup, int buttonId)
        {
            popup.Finish();
            popup.NotifyPopupDelegate(buttonId);
        }

        private void NotifyPopupDelegate(int buttonId)
        {
            if (popupDelegate != null)
            {
                popupDelegate(this, buttonId);
            }
        }
    }
}

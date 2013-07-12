using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BomberEngine.Core.Assets.Types;
using BomberEngine.Core.Events;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual.UI;

namespace BomberEngine.Core.Visual
{
    public delegate void PopupListDelegate(PopupList list);

    public class PopupList : View
    {
        public PopupListDelegate popupDelegate;
        public int selectedIndex = -1;

        public PopupList(Font font, String[] names, int minWidth)
        {
            RectView back = new RectView(0, 0, 100, 100, Color.Gray, Color.Black);
            AddView(back);

            View contentView = new View(100, 100);

            int w = minWidth;
            for (int i = 0; i < names.Length; ++i)
            {
                ListItem item = new ListItem(font, names[i], w, font.FontHeight());
                item.id = i;
                item.SetDelegate(OnItemSelected);
                contentView.AddView(item);
            }

            contentView.LayoutVer(2);
            contentView.ResizeToFitViewsVer();

            AddView(contentView);

            back.width = contentView.width;
            back.height = contentView.height;

            width = contentView.width;
            height = contentView.height;
        }

        public override bool HandleEvent(Event evt)
        {
            if (evt.code == Event.KEY)
            {
                KeyEvent keyEvent = evt as KeyEvent;
                if (keyEvent.arg.key == KeyCode.Escape)
                {
                    if (keyEvent.state == KeyState.Pressed)
                    {
                        Hide();
                    }
                    return true;
                }
            }

            return base.HandleEvent(evt);
        }

        private void OnItemSelected(Button button)
        {
            Hide();

            if (popupDelegate != null)
            {
                selectedIndex = button.id;
                popupDelegate(this);
            }
        }

        private void Hide()
        {
            parent.RemoveView(this);
        }
    }

    class ListItem : Button
    {   
        private string text;

        private RectView backView;
        private TextView textView;

        public ListItem(Font font, String text, int width, int height)
            : base(width, height)
        {   
            this.text = text;

            backView = new RectView(0, 0, width, height, Color.Black, Color.Black);
            backView.visible = false;
            AddView(backView);

            textView = new TextView(font, text);
            textView.alignX = View.ALIGN_CENTER;
            textView.x = 0.5f * width;
            AddView(textView);
        }

        protected override void OnFocusChanged(bool focused)
        {
            if (focused)
            {
                textView.color = Color.Yellow;
                backView.visible = true;
            }
            else
            {
                textView.color = Color.White;
                backView.visible = false;
            }
        }
    }
}

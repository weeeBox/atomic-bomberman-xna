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
        public PopupList(Font font, String[] names, int minWidth)
        {
            RectView back = new RectView(0, 0, 100, 100, Color.Gray, Color.Black);
            AddView(back);

            View contentView = new View(100, 100);

            int w = minWidth;
            int h = 20;
            for (int i = 0; i < names.Length; ++i)
            {
                contentView.AddView(new ListItem(font, i, names[i], w, font.FontHeight()));
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
                    return true;
                }
            }

            return base.HandleEvent(evt);
        }
    }

    class ListItem : Button
    {
        private int index;
        private string text;

        private RectView backView;
        private TextView textView;

        public ListItem(Font font, int index, String text, int width, int height)
            : base(width, height)
        {
            this.index = index;
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

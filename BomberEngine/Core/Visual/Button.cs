using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Assets.Types;
using Microsoft.Xna.Framework;
using BomberEngine.Game;

namespace BomberEngine.Core.Visual
{
    public class Button : AbstractButton
    {
        private TextView label;
        private Font font;

        public Color normalColor = Color.White;
        public Color highlightedColor = Color.Yellow;

        public Button(String text, int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            font = Application.Assets().SystemFont;
            SetText(text);
        }

        public Button(String text, Font font, int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            this.font = font;
            SetText(text);
        }

        public void SetText(String text)
        {
            if (text != null && text.Length > 0)
            {
                if (label != null)
                {
                    if (label.Parent() != this)
                    {
                        AddView(label);
                    }
                    label.SetText(text);
                }
                else
                {
                    label = new TextView(font, text);
                    AddView(label);
                }
            }
            else
            {
                if (label != null)
                {
                    RemoveView(label);
                }
            }
        }

        protected override void OnFocusChanged(bool focused)
        {
            color = focused ? highlightedColor : normalColor;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Assets.Types;
using Microsoft.Xna.Framework;
using BomberEngine.Game;
using BomberEngine.Core.Visual.UI;

namespace BomberEngine.Core.Visual
{
    public class TextButton : Button
    {
        private TextView label;
        private Font font;

        public Color normalColor = Color.White;
        public Color highlightedColor = Color.Yellow;

        public TextButton(String text, Font font, float x, float y, float width, float height)
            : base(x, y, width, height)
        {
            this.font = font;
            AddBackground();
            SetText(text);
        }

        private void AddBackground()
        {
            Color color = Color.Black;
            RectView rect = new RectView(0, 0, width, height, color, color);
            AddView(rect);
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
                    label.x = 0.5f * (width - label.width);
                    label.y = 0.5f * (height - label.height);
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

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public TextView Label()
        {
            return label;
        }

        public String Text()
        {
            return label != null && label.Parent() == this ? label.text : null;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;
using BomberEngine.Util;
using BomberEngine.Core.Assets.Types;

namespace BomberEngine.Core.Visual
{
    public enum TextAlign
    {
        LEFT = 1 << 0,
        HCENTER = 1 << 1,
        RIGHT = 1 << 2,
        TOP = 1 << 3,
        VCENTER = 1 << 4,
        BOTTOM = 1 << 5
    }

    public class TextView : View
    {
        protected struct FormattedString
        {
            public String text;
            public float width;

            public FormattedString(String text, float width)
            {
                this.text = text;
                this.width = width;
            }
        }

        public String text;
        public Font font;

        protected FormattedString[] formattedStrings;

        public TextView(Font font, String text)
        {
            Debug.Assert(font != null);

            this.font = font;
            SetString(text);
        }

        public void SetString(String newString)
        {
            SetString(newString, Int32.MaxValue);
        }

        public virtual void SetString(String newString, int wrapWidth)
        {
            text = newString;

            String[] strings = font.WrapString(text, wrapWidth);
            int stringsCount = strings.Length;
            formattedStrings = new FormattedString[stringsCount];
            for (int i = 0; i < stringsCount; ++i)
            {
                String str = strings[i];
                int strWidth = font.StringWidth(str);
                if (strWidth > width)
                    width = strWidth;
                formattedStrings[i] = new FormattedString(str, strWidth);
            }
            height = (font.FontHeight() + font.LineOffset()) * formattedStrings.Length - font.LineOffset();
        }

        public String getString()
        {
            return text;
        }

        public void setAlign(TextAlign textAlign)
        {   
            if ((textAlign & TextAlign.RIGHT) != 0)
            {
                alignX = ALIGN_MAX;
            }
            else if ((textAlign & TextAlign.HCENTER) != 0)
            {
                alignX = ALIGN_CENTER;
            }
            else
            {
                alignX = ALIGN_MIN;
            }

            if ((textAlign & TextAlign.BOTTOM) != 0)
            {
                alignY = ALIGN_MAX;
            }
            else if ((textAlign & TextAlign.VCENTER) != 0)
            {
                alignY = ALIGN_CENTER;
            }
            else
            {
                alignY = ALIGN_MIN;
            }
        }

        public override void Draw(Context context)
        {
            PreDraw(context);

            float dx;
            float dy = 0;
            int itemHeight = font.FontHeight();

            for (int i = 0; i < formattedStrings.Length; i++)
            {
                FormattedString str = formattedStrings[i];
                dx = alignX * (width - str.width);
                font.DrawString(context, str.text, dx, dy);
                dy += itemHeight + font.LineOffset();
            }

            PostDraw(context);
        }
    }
}

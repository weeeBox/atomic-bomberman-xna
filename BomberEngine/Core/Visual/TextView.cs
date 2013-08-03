using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;
using BomberEngine.Util;
using BomberEngine.Core.Assets.Types;
using Microsoft.Xna.Framework;

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

        private String m_text;
        private Font m_font;
        private int m_wrapWidth;

        private Color m_backColor;
        private bool m_hasBackColor;

        protected FormattedString[] formattedStrings;

        public TextView(Font font, String text)
            : this(font, text, Int32.MaxValue)
        {
        }

        public TextView(Font font, String text, int wrapWidth = Int32.MaxValue)
        {
            Debug.Assert(font != null);

            m_font = font;
            SetText(text, wrapWidth);
        }

        public virtual void SetText(String newString)
        {
            SetText(newString, m_wrapWidth);
        }

        public virtual void SetText(String newString, int wrapWidth)
        {
            m_text = newString;
            m_wrapWidth = wrapWidth;

            String[] strings = m_font.WrapString(m_text, wrapWidth);
            int stringsCount = strings.Length;
            formattedStrings = new FormattedString[stringsCount];
            for (int i = 0; i < stringsCount; ++i)
            {
                String str = strings[i];
                int strWidth = m_font.StringWidth(str);
                if (strWidth > width)
                    width = strWidth;
                formattedStrings[i] = new FormattedString(str, strWidth);
            }
            height = (m_font.FontHeight() + m_font.LineOffset()) * formattedStrings.Length - m_font.LineOffset();
        }

        public String getString()
        {
            return m_text;
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
            int itemHeight = m_font.FontHeight();

            if (m_hasBackColor)
            {
                context.FillRect(0, 0, width, height, m_backColor);
            }

            for (int i = 0; i < formattedStrings.Length; i++)
            {
                FormattedString str = formattedStrings[i];
                dx = alignX * (width - str.width);
                m_font.DrawString(context, str.text, dx, dy);
                dy += itemHeight + m_font.LineOffset();
            }

            PostDraw(context);
        }

        public String text
        {
            get { return m_text; }
            set
            {
                SetText(value);
            }
        }

        public Color backColor
        {
            get { return m_backColor; }
            set
            {
                m_hasBackColor = value.A > 0.0f;
                m_backColor = value;
            }
        }
    }
}

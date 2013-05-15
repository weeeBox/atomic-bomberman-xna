using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BomberEngine.Core.Visual
{
    using XnaSpriteFont = Microsoft.Xna.Framework.Graphics.SpriteFont;

    public class VectorFont : IFont
    {
        private XnaSpriteFont fnt;
        private int fntHeight;

        public VectorFont(XnaSpriteFont fnt)
        {
            this.fnt = fnt;
            fntHeight = (int)fnt.MeasureString("0").Y;            
        }

        public string[] WrapString(string text, int wrapWidth)
        {
            throw new NotImplementedException();
        }

        public int StringWidth(string str)
        {
            return (int)(fnt.MeasureString(str).X);
        }

        public int FontHeight()
        {
            return fntHeight;
        }

        public int LineOffset()
        {
            return fnt.LineSpacing;
        }

        public void DrawString(Context context, String text, float x, float y)
        {
            DrawString(context, text, x, y, TextAlign.LEFT | TextAlign.TOP);
        }

        public void DrawString(Context context, String text, float x, float y, TextAlign textAlign)
        {
            float dx = x;
            float dy = y;

            if ((textAlign & TextAlign.RIGHT) != 0)
            {
                dx -= StringWidth(text);
            }
            else if ((textAlign & TextAlign.HCENTER) != 0)
            {
                dx -= 0.5f * StringWidth(text);
            }
            if ((textAlign & TextAlign.BOTTOM) != 0)
            {
                dy -= FontHeight();
            }
            else if ((textAlign & TextAlign.VCENTER) != 0)
            {
                dy -= 0.5f * FontHeight();
            }

            context.DrawString(fnt, dx, dy, text);
        }
    }
}

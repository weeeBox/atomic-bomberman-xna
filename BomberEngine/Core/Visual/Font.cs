using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Assets;

namespace BomberEngine.Core.Visual
{
    public abstract class Font : Asset
    {
        public abstract string[] WrapString(String text, int wrapWidth);
        public abstract int StringWidth(String str);
        public abstract int FontHeight();
        public abstract int LineOffset();

        public abstract void DrawString(Context context, String text, float x, float y);
        public abstract void DrawString(Context context, String text, float x, float y, TextAlign textAlign);
    }
}

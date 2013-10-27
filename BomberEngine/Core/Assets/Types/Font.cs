using System;

namespace BomberEngine
{
    public abstract class Font : Asset
    {
        public abstract string[] WrapString(String text, int wrapWidth);
        public abstract int StringWidth(String str);
        public abstract int CharWidth(char chr);
        public abstract int FontHeight();
        public abstract int LineOffset();

        public abstract void DrawString(Context context, String text, float x, float y);
        public abstract void DrawString(Context context, String text, float x, float y, TextAlign textAlign);
    }
}

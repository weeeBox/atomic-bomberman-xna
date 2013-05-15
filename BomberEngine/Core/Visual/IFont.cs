using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Visual
{
    public interface IFont
    {
        string[] WrapString(String text, int wrapWidth);
        int StringWidth(String str);
        int FontHeight();
        int LineOffset();

        void DrawString(String text, float x, float y);
        void DrawString(String text, float x, float y, TextAlign textAlign);
    }
}

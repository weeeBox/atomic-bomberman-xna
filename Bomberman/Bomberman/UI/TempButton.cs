using System;
using BomberEngine;
using Bomberman.Game;

namespace Bomberman.UI
{
    public class TempButton : TextButton
    {
        private const int DefaultWidth = 150;

        public TempButton(String text)
            : base(text, DefaultFont, 0, 0, DefaultWidth, DefaultFont.FontHeight())
        {
        }

        private static Font DefaultFont
        {
            get { return Helper.fontButton; }
        }
    }
}

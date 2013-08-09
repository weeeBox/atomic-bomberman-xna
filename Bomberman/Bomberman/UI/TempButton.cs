using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Visual;
using Bomberman.Game;
using BomberEngine.Core.Assets.Types;

namespace Bomberman.UI
{
    public class TempButton : TextButton
    {
        private const int DefaultWidth = 100;

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

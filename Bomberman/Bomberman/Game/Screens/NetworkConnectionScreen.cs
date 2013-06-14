using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using Assets;
using BomberEngine.Core.Assets.Types;

namespace Bomberman.Game.Screens
{
    public class NetworkConnectionScreen : Screen
    {
        public NetworkConnectionScreen()
        {
            Font font = Helper.GetFont(A.fnt_button);

            TextView textView = new TextView(font, "Connecting to the server...");
            textView.x = 0.5f * (width - textView.width);
            textView.y = 0.5f * (height - textView.height);

            AddView(textView);
        }
    }
}

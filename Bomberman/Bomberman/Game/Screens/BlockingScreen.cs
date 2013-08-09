using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Operations;
using BomberEngine.Core.Visual.UI;
using Microsoft.Xna.Framework;
using Bomberman.UI;
using Bomberman.Game;

namespace BomberEngine.Core.Visual
{
    public class BlockingScreen : Screen
    {
        public BlockingScreen(String text, ButtonDelegate buttonDelegate = null)
            : this(new TextView(Helper.fontButton, text), buttonDelegate)
        {
        }

        public BlockingScreen(View messageView, ButtonDelegate buttonDelegate = null)
        {
            AllowsDrawPrevious = true;
            AllowsUpdatePrevious = true;

            View contentView = new RectView(0, 0, 366, 182, Color.Black, Color.White);
            contentView.x = 0.5f * width;
            contentView.y = 0.5f * height;
            contentView.alignX = contentView.alignY = View.ALIGN_CENTER;

            messageView.x = 0.5f * contentView.width;
            messageView.y = 0.5f * contentView.height;
            messageView.alignX = messageView.alignY = View.ALIGN_CENTER;
            contentView.AddView(messageView);

            Button cancelButton = new TempButton("Cancel");
            cancelButton.x = 0.5f * contentView.width;
            cancelButton.y = contentView.height - 12;
            cancelButton.alignX = View.ALIGN_CENTER;
            cancelButton.alignY = View.ALIGN_MAX;
            cancelButton.buttonDelegate = buttonDelegate;
            SetCancelButton(cancelButton);

            contentView.AddView(cancelButton);

            AddView(contentView);
        }
    }
}

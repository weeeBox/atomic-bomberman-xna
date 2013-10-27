using BomberEngine;
using Bomberman.UI;

namespace Bomberman.Game.Screens
{
    public class RoundResultScreen : BaseResultScreen
    {
        public RoundResultScreen(Game game, ButtonDelegate buttonDelegate) : 
            base(game)
        {
            // buttons
            View buttons = new View(0.5f * contentView.width, contentView.height, 0, 0);
            buttons.alignX = View.ALIGN_CENTER;
            buttons.alignY = View.ALIGN_MAX;

            Button button = new TempButton("EXIT");
            button.id = (int)ButtonId.Exit;
            button.buttonDelegate = buttonDelegate;
            SetCancelButton(button);
            buttons.AddView(button);

            button = new TempButton("START!");
            button.id = (int)ButtonId.Continue; ;
            button.buttonDelegate = buttonDelegate;
            FocusView(button);

            SetConfirmButton(button);
            buttons.AddView(button);

            buttons.LayoutHor(20);
            buttons.ResizeToFitViews();
            contentView.AddView(buttons);
        }
    }
    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Operations;
using BomberEngine.Core.Visual.UI;
using Microsoft.Xna.Framework;

namespace BomberEngine.Core.Visual
{
    public class BlockingScreen : Screen
    {
        private View m_contentView;
        
        public BlockingScreen(BaseOperation op, ButtonDelegate buttonDelegate)
        {
            AllowsDrawPrevious = true;
            AllowsUpdatePrevious = true;

            m_contentView = new RectView(0, 0, 366, 182, Color.Black, Color.White);
            m_contentView.x = 0.5f * width;
            m_contentView.y = 0.5f * height;
            m_contentView.alignX = m_contentView.alignY = View.ALIGN_CENTER;

            
        }
    }
}

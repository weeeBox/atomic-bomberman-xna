using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Assets.Types;
using BomberEngine.Game;

namespace BomberEngine.Demo
{
    public class DemoDebugView : View
    {
        private TextView m_tickView;

        public DemoDebugView(Font font)
        {   
            m_tickView = new TextView(font, "Tick: 9999999999");
            AddView(m_tickView);

            LayoutVer(0);
            ResizeToFitViews();
        }

        public override void Update(float delta)
        {   
            base.Update(delta);
            m_tickView.SetText("Tick: " + Application.tickIndex);
        }
    }
}

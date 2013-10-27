
namespace BomberEngine
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

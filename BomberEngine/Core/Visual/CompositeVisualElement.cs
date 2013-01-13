using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Visual
{
    public class CompositeVisualElement : VisualElement
    {
        private List<VisualElement> children;

        public CompositeVisualElement()
            : this(0, 0, 0, 0)
        {
        }

        public CompositeVisualElement(int width, int height)
            : this(0, 0, width, height)
        {
        }

        public CompositeVisualElement(float x, float y, int width, int height)
            : base(x, y, width, height)
        {
            children = new List<VisualElement>();
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            foreach (VisualElement child in children)
            {
                child.Update(delta);
            }
        }

        public override void PostDraw(Context context)
        {
            foreach (VisualElement child in children)
            {
                child.Draw(context);
            }

            RestoreTransformations(context);
        }

        public virtual void AddChild(VisualElement child)
        {
            child.SetParent(this);
            children.Add(child);
        }
    }
}

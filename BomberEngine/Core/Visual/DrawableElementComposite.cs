using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Visual
{
    public class DrawableElementComposite : DrawableElement
    {
        private List<DrawableElement> children;

        public DrawableElementComposite()
            : this(0, 0, 0, 0)
        {
        }

        public DrawableElementComposite(int width, int height)
            : this(0, 0, width, height)
        {
        }

        public DrawableElementComposite(float x, float y, int width, int height)
            : base(x, y, width, height)
        {
            children = new List<DrawableElement>();
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            foreach (DrawableElement child in children)
            {
                child.Update(delta);
            }
        }

        public override void PostDraw(Context context)
        {
            foreach (DrawableElement child in children)
            {
                child.Draw(context);
            }

            RestoreTransformations(context);
        }

        public virtual void AddChild(DrawableElement child)
        {
            child.SetParent(this);
            children.Add(child);
        }
    }
}

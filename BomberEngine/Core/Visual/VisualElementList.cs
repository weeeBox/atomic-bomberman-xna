using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Visual
{
    public class VisualElementList : BaseUpdatableList<VisualElement>, IDrawable
    {
        public static readonly VisualElementList Null = new NullGameObjectList();
        private static readonly VisualElement nullElement = new NullGameObject();

        public VisualElementList()
            : base(nullElement)
        {
        }

        public VisualElementList(int capacity)
            : base(nullElement, capacity)
        {
        }

        public virtual void Draw(Context context)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                list[i].Draw(context);
            }
        }
    }

    internal sealed class NullGameObjectList : VisualElementList
    {
        public override void Update(float delta)
        {
        }

        public override void Draw(Context context)
        {
        }

        public override bool Add(VisualElement updatable)
        {
            throw new InvalidOperationException("Can't add element to unmodifiable game object list");
        }

        public override bool Remove(VisualElement updatable)
        {
            throw new InvalidOperationException("Can't remove element from unmodifiable game object list");
        }

        public override void Remove(int index)
        {
            throw new InvalidOperationException("Can't remove element from unmodifiable game object list");
        }

        public override void Clear()
        {
            throw new InvalidOperationException("Can't clear unmodifiable game object list");
        }

        public override int Count()
        {
            return 0;
        }

        public override bool Contains(VisualElement updatable)
        {
            return false;
        }

        public override bool IsNull()
        {
            return true;
        }
    }

    internal sealed class NullGameObject : VisualElement
    {
        public override void Update(float delta)
        {   
        }

        public override void Draw(Context context)
        {   
        }
    }
}

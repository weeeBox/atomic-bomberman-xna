using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Visual
{
    public class GameObjectList : BaseUpdatableList<GameObject>, IDrawable
    {
        private static readonly GameObject nullElement = new NullGameObject();

        public GameObjectList()
            : base(nullElement)
        {
        }

        public GameObjectList(int capacity)
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

    internal sealed class NullGameObjectList : GameObjectList
    {
        public override void Update(float delta)
        {
        }

        public override void Draw(Context context)
        {
        }

        public override bool Add(GameObject updatable)
        {
            throw new InvalidOperationException("Can't add element to unmodifiable game object list");
        }

        public override bool Remove(GameObject updatable)
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

        public override bool Contains(GameObject updatable)
        {
            return false;
        }

        public override bool IsNull()
        {
            return true;
        }
    }

    internal sealed class NullGameObject : GameObject
    {
        public override void Update(float delta)
        {   
        }

        public override void Draw(Context context)
        {   
        }
    }
}

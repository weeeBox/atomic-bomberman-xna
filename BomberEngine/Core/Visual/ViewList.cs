using System;
using System.Collections.Generic;

namespace BomberEngine
{
    public class ViewList : BaseUpdatableList<View>, IDrawable
    {
        public static readonly ViewList Null = new NullViewList();
        private static readonly View nullView = new NullView();

        public ViewList()
            : base(nullView)
        {
        }

        public ViewList(int capacity)
            : base(nullView, capacity)
        {
        }

        protected ViewList(List<View> list, View nullUpdatable)
            : base(list, nullUpdatable)
        {
        }

        public virtual void Draw(Context context)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                View view = list[i];
                if (view.visible)
                {
                    view.Draw(context);
                }
            }
        }
    }

    internal sealed class NullViewList : ViewList
    {
        public NullViewList()
            : base(null, null)
        {
        }

        public override void Update(float delta)
        {
        }

        public override void Draw(Context context)
        {
        }

        public override bool Add(View updatable)
        {
            throw new InvalidOperationException("Can't add element to null list");
        }

        public override bool Remove(View updatable)
        {
            throw new InvalidOperationException("Can't remove element from null list");
        }

        public override View Get(int index)
        {
            throw new InvalidOperationException("Can't get element from null list");
        }

        public override int IndexOf(View updatable)
        {
            return -1;
        }

        public override void RemoveAt(int index)
        {
            throw new InvalidOperationException("Can't remove element from null list");
        }

        public override void Clear()
        {
            throw new InvalidOperationException("Can't clear null list");
        }

        public override int Count()
        {
            return 0;
        }

        public override bool Contains(View updatable)
        {
            return false;
        }

        public override bool IsNull()
        {
            return true;
        }
    }

    internal sealed class NullView : View
    {
        public override void Update(float delta)
        {   
        }

        public override void Draw(Context context)
        {   
        }
    }
}

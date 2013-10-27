using System;
using System.Collections.Generic;

namespace BomberEngine
{
    public delegate void UpdatableDelegate(float delta);

    public class UpdatableList : BaseUpdatableList<IUpdatable>, IDestroyable
    {
        public static readonly UpdatableList Null = new NullUpdatableList();
        private static readonly IUpdatable nullUpdatable = new NullUpdatable();

        public UpdatableList()
            : base(nullUpdatable) 
        {
        }

        public UpdatableList(int capacity)
            : base(nullUpdatable, capacity)
        {
        }

        protected UpdatableList(List<IUpdatable> list, IUpdatable nullUpdatable)
            : base(list, nullUpdatable)
        {
        }

        public void Destroy()
        {
            if (Count() > 0)
            {
                Clear();
            }
        }
    }

    internal sealed class NullUpdatableList : UpdatableList
    {
        public NullUpdatableList()
            : base(null, null)
        {
        }

        public override void Update(float delta)
        {
        }

        public override bool Add(IUpdatable updatable)
        {
            throw new InvalidOperationException("Can't add element to unmodifiable updatable list");
        }

        public override bool Remove(IUpdatable updatable)
        {
            throw new InvalidOperationException("Can't remove element from unmodifiable updatable list");
        }

        public override void RemoveAt(int index)
        {
            throw new InvalidOperationException("Can't remove element from unmodifiable updatable list");
        }

        public override void Clear()
        {
            throw new InvalidOperationException("Can't clear unmodifiable updatable list");
        }

        public override int Count()
        {
            return 0;
        }

        public override bool Contains(IUpdatable updatable)
        {
            return false;
        }

        public override bool IsNull()
        {
            return true;
        }
    }

    internal sealed class NullUpdatable : IUpdatable
    {
        public void Update(float delta)
        {
        }
    }
}

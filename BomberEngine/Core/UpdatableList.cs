using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Core
{
    public class UpdatableList : BaseUpdatableList<IUpdatable>
    {
        public static readonly UpdatableList Empty = new NullUpdatableList();
        private static readonly IUpdatable nullElement = new NullUpdatable();

        public UpdatableList()
            : base(nullElement) 
        {
        }

        public UpdatableList(int capacity)
            : base(nullElement, capacity)
        {
        }
    }

    internal sealed class NullUpdatableList : UpdatableList
    {
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

        public override void Remove(int index)
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

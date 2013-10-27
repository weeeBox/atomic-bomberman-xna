using System;
using System.Collections.Generic;

namespace BomberEngine
{
    public class EventHandlerList : BaseList<IEventHandler>, IEventHandler
    {
        public static readonly EventHandlerList Null = new NullEventHandlerList();
        private static readonly IEventHandler nullHandler = new NullEventHandler();

        public EventHandlerList()
            : base(nullHandler)
        {
        }

        public EventHandlerList(int capacity)
            : base(nullHandler, capacity)
        {
        }

        protected EventHandlerList(List<IEventHandler> list, IEventHandler nullElement)
            : base(list, nullElement)
        {
        }

        public virtual bool HandleEvent(Event evt)
        {
            bool handled = false;

            int elementCount = list.Count;
            for (int i = 0; i < elementCount; ++i)
            {
                handled |= list[i].HandleEvent(evt);
            }
            ClearRemoved();

            return handled;
        }
    }

    internal class NullEventHandlerList : EventHandlerList
    {
        public NullEventHandlerList()
            : base(null, null)
        {
        }

        public override bool HandleEvent(Event evt)
        {
            return false;
        }

        public override bool Add(IEventHandler handler)
        {
            throw new InvalidOperationException("Can't add element to unmodifiable handler list");
        }

        public override bool Remove(IEventHandler handler)
        {
            throw new InvalidOperationException("Can't remove element from unmodifiable handler list");
        }

        public override void RemoveAt(int index)
        {
            throw new InvalidOperationException("Can't remove element from unmodifiable handler list");
        }

        public override void Clear()
        {
            throw new InvalidOperationException("Can't clear unmodifiable handler list");
        }

        public override int Count()
        {
            return 0;
        }

        public override bool Contains(IEventHandler handler)
        {
            return false;
        }

        public override bool IsNull()
        {
            return true;
        }
    }

    internal class NullEventHandler : IEventHandler
    {
        public bool HandleEvent(Event evt)
        {
            return false;
        }
    }
}

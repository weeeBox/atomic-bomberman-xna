using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Core
{
    public class UpdatableList : IUpdatable, Collection<IUpdatable>
    {
        public static readonly UpdatableList Empty = new UnmmodifiableUpdatableList();

        private static IUpdatable NULL_UPDATABLE = new NullUpdatable();

        private List<IUpdatable> list;
        private int removedCount;

        public UpdatableList()
        {
            list = new List<IUpdatable>();
        }

        public UpdatableList(int capacity)
        {
            list = new List<IUpdatable>(capacity);
        }

        public virtual void Update(float delta)
        {
            int elementsCount = list.Count;
            for (int i = 0; i < elementsCount; ++i) // do not update added items on that tick
            {
                list[i].Update(delta);
            }

            if (removedCount > 0)
            {
                ClearRemoved();
            }
        }

        public virtual bool Add(IUpdatable updatable)
        {
            list.Add(updatable);
            return true;
        }

        public virtual bool Remove(IUpdatable updatable)
        {
            int index = list.IndexOf(updatable);
            if (index != -1)
            {
                Remove(index);
                return true;
            }

            return false;
        }

        public virtual void Remove(int index)
        {
            ++removedCount;
            list[index] = NULL_UPDATABLE;
        }

        public virtual void RemoveLast()
        {
            if (list.Count > 0)
            {
                Remove(list.Count - 1);
            }
        }

        public virtual void Clear()
        {
            list.Clear();
        }

        public virtual int Count()
        {
            return list.Count - removedCount;
        }

        public virtual bool Contains(IUpdatable updatable)
        {
            return list.Contains(updatable);
        }

        private void ClearRemoved()
        {   
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i] == NULL_UPDATABLE)
                {
                    list.RemoveAt(i);
                    --removedCount;

                    if (removedCount == 0)
                    {
                        break;
                    }
                }
            }
        }
    }

    sealed class UnmmodifiableUpdatableList : UpdatableList
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
    }

    sealed class NullUpdatable : IUpdatable
    {
        public void Update(float delta)
        {
        }
    }
}

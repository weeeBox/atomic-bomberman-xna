using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Core
{
    public class UpdatableList : IUpdatable, Collection<IUpdatable>
    {
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

        public void Update(float delta)
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

        public bool Add(IUpdatable updatable)
        {
            list.Add(updatable);
            return true;
        }

        public bool Remove(IUpdatable updatable)
        {
            int index = list.IndexOf(updatable);
            if (index != -1)
            {
                Remove(index);
                return true;
            }

            return false;
        }

        public void Remove(int index)
        {
            ++removedCount;
            list[index] = NULL_UPDATABLE;
        }

        public void RemoveLast()
        {
            if (list.Count > 0)
            {
                Remove(list.Count - 1);
            }
        }

        public void SetLast(IUpdatable updatable)
        {
            if (list.Count > 0)
            {
                list[list.Count - 1] = updatable;
            }
            else
            {
                Add(updatable);
            }
        }

        public void Clear()
        {
            list.Clear();
        }

        public int Count()
        {
            return list.Count - removedCount;
        }

        public bool Contains(IUpdatable updatable)
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
}

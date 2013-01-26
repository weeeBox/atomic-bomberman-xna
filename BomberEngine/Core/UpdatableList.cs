using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Core
{
    public class UpdatableList : Updatable, Collection<Updatable>
    {
        private static Updatable NULL_UPDATABLE = new NullUpdatable();

        private List<Updatable> list;
        private int removedCount;

        public UpdatableList()
        {
            list = new List<Updatable>();
        }

        public UpdatableList(int capacity)
        {
            list = new List<Updatable>(capacity);
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

        public bool Add(Updatable updatable)
        {
            list.Add(updatable);
            return true;
        }

        public bool Remove(Updatable updatable)
        {
            int index = list.IndexOf(updatable);
            if (index != -1)
            {
                ++removedCount;

                list[index] = NULL_UPDATABLE;
                return true;
            }

            return false;
        }

        public void Clear()
        {
            list.Clear();
        }

        public int Count()
        {
            return list.Count - removedCount;
        }

        public bool Contains(Updatable updatable)
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

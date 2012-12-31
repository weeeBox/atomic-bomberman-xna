using System.Collections.Generic;
using System;

namespace BomberEngine.Core
{
    public class UpdatableList : Updatable, Composite<Updatable>
    {
        private List<Updatable> list;

        private bool insideLoop;
        private bool insideLoopObjectWereDeleted;

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
            insideLoop = true;

            int count = list.Count; // remember the list's size here: during the loop we may add more objects
            for (int i = 0; i < count; ++i)
            {
                Updatable item = list[i];
                if (item != null)
                {
                    item.Update(delta);
                }
            }

            insideLoop = false;

            if (insideLoopObjectWereDeleted)
            {
                RemoveDeleted();
                insideLoopObjectWereDeleted = false;
            }
        }

        public void Add(Updatable item)
        {
            list.Add(item);
        }

        public void Remove(Updatable item)
        {
            int index = list.IndexOf(item);
            if (index != -1)
            {
                if (insideLoop)
                {
                    list[index] = null; // we can't remove it now, because it may screw the update loop
                    insideLoopObjectWereDeleted = true;
                }
                else
                {
                    list.RemoveAt(index);
                }
            }
        }

        public int Count()
        {
            return list.Count;
        }

        private void RemoveDeleted()
        {
            int newCount = 0;
            int oldCount = list.Count;

            for (int to = 0, from = 0; to < oldCount && from < oldCount; ++to)
            {
                if (list[to] == null)
                {
                    from = Math.Max(to + 1, from);
                    for (; from < oldCount; ++from)
                    {
                        Updatable item = list[from];
                        if (item != null)
                        {
                            list[to] = item;
                            list[from] = null;

                            ++newCount;
                            break;
                        }
                    }
                }
                else
                {
                    ++newCount;
                }
            }

            if (newCount != oldCount)
            {
                list.RemoveRange(newCount, oldCount - newCount);
            }
        }
    }
}

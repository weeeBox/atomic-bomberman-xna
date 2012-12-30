using System.Collections.Generic;

namespace core
{
    public class UpdatableList : Updatable, Composite<Updatable>
    {
        private List<Updatable> list;
        
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
            int index = 0;
            int count = list.Count; // remember the list's size here: during the loop we may add more objects
            while (index < count)
            {
                Updatable item = list[index];
                if (item == null)
                {
                    list.RemoveAt(index);
                    --count;
                    continue;
                }

                item.Update(delta);
                ++index;
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
                list[index] = null; // we can't remove it now, because it may screw the update loop
            }
        }

        public int Count()
        {
            return list.Count;
        }
    }
}

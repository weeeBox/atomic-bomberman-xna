using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Core
{
    public class UpdatableList : Updatable, Collection<Updatable>
    {
        private ObjectsList<Updatable> list;

        public UpdatableList()
        {
            list = new ObjectsList<Updatable>();
        }

        public UpdatableList(int capacity)
        {
            list = new ObjectsList<Updatable>(capacity);
        }

        public void Update(float delta)
        {
            foreach (Updatable updatable in list)
            {
                updatable.Update(delta);
            }
        }

        public bool Add(Updatable updatable)
        {
            list.Add(updatable);
            return true;
        }

        public bool Remove(Updatable updatable)
        {
            return list.Remove(updatable);
        }

        public void Clear()
        {
            list.Clear();
        }

        public int Count()
        {
            return list.Count;
        }
    }
}

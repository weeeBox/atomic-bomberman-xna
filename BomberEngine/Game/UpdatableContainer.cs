using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;

namespace BomberEngine.Game
{
    public class UpdatableContainer : IUpdatable
    {
        private UpdatableList list;

        public UpdatableContainer()
        {
            list = new UpdatableList();
        }

        public void Update(float delta)
        {
            list.Update(delta);
        }

        protected void AddUpdatable(IUpdatable updatable)
        {
            list.Add(updatable);
        }

        protected void RemoveUpdatable(IUpdatable updatable)
        {
            list.Remove(updatable);
        }

        protected UpdatableList GetUpdatableList()
        {
            return list;
        }
    }
}

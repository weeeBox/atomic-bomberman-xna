using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;
using BomberEngine.Core.Visual;

namespace BomberEngine.Core
{
    public class DrawableList : IDrawable, Collection<IDrawable>
    {
        private ObjectsList<IDrawable> list;

        public DrawableList()
        {
            list = new ObjectsList<IDrawable>();
        }

        public DrawableList(int capacity)
        {
            list = new ObjectsList<IDrawable>(capacity);
        }

        public void Draw(Context context)
        {
            foreach (IDrawable drawable in list)
            {
                drawable.Draw(context);
            }
        }

        public bool Add(IDrawable drawable)
        {
            list.Add(drawable);
            return true;
        }

        public bool Remove(IDrawable drawable)
        {
            return list.Remove(drawable);
        }

        public void Clear()
        {
            list.Clear();
        }

        public int Count()
        {
            return list.Count;
        }

        public bool Contains(IDrawable drawable)
        {
            return list.Contains(drawable);
        }
    }
}

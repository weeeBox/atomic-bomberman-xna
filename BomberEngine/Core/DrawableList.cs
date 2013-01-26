using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;
using BomberEngine.Core.Visual;

namespace BomberEngine.Core
{
    public class DrawableList : Drawable, Collection<Drawable>
    {
        private ObjectsList<Drawable> list;

        public DrawableList()
        {
            list = new ObjectsList<Drawable>();
        }

        public DrawableList(int capacity)
        {
            list = new ObjectsList<Drawable>(capacity);
        }

        public void Draw(Context context)
        {
            foreach (Drawable drawable in list)
            {
                drawable.Draw(context);
            }
        }

        public bool Add(Drawable drawable)
        {
            list.Add(drawable);
            return true;
        }

        public bool Remove(Drawable drawable)
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

        public bool Contains(Drawable drawable)
        {
            return list.Contains(drawable);
        }
    }
}

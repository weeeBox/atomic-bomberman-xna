using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    public class DrawableList : Drawable, Composite<Drawable>
    {
        private List<Drawable> list;

        public DrawableList()
        {
            list = new List<Drawable>();
        }

        public DrawableList(int capacity)
        {
            list = new List<Drawable>(capacity);
        }

        public void Draw(Context context)
        {
            foreach (Drawable drawable in list)
            {
                drawable.Draw(context);
            }
        }

        public void Add(Drawable t)
        {
            list.Add(t);
        }

        public void Remove(Drawable t)
        {
            list.Remove(t);
        }

        public int Count()
        {
            return list.Count;
        }
    }
}

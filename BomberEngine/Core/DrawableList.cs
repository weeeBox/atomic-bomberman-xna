using System;

namespace BomberEngine
{
    public class DrawableList : IDrawable, IBaseCollection<IDrawable>
    {
        public static readonly DrawableList Null = new NullDrawbleList();

        private ObjectsList<IDrawable> list;

        public DrawableList()
        {
            list = new ObjectsList<IDrawable>();
        }

        public DrawableList(int capacity)
        {
            list = new ObjectsList<IDrawable>(capacity);
        }

        public virtual void Draw(Context context)
        {
            foreach (IDrawable drawable in list)
            {
                drawable.Draw(context);
            }
        }

        public virtual bool Add(IDrawable drawable)
        {
            list.Add(drawable);
            return true;
        }

        public virtual bool Remove(IDrawable drawable)
        {
            return list.Remove(drawable);
        }

        public virtual void Clear()
        {
            list.Clear();
        }

        public virtual int Count()
        {
            return list.Count;
        }

        public virtual bool Contains(IDrawable drawable)
        {
            return list.Contains(drawable);
        }

        public virtual bool IsNull()
        {
            return false;
        }
    }

    sealed class NullDrawbleList : DrawableList
    {
        public override void Draw(Context context)
        {   
        }

        public override bool Add(IDrawable drawable)
        {
            throw new InvalidOperationException("Can't add element to unmodifiable drawable list");
        }

        public override bool Remove(IDrawable drawable)
        {
            throw new InvalidOperationException("Can't remove element from unmodifiable drawable list");
        }

        public override void Clear()
        {
            throw new InvalidOperationException("Can't clear unmodifiable drawable list");
        }

        public override int Count()
        {
            return 0;
        }

        public override bool Contains(IDrawable drawable)
        {
            return false;
        }

        public override bool IsNull()
        {
            return true;
        }
    }
}
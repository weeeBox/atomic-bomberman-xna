
namespace BomberEngine
{
    internal interface IObjectsPool
    {
        void Recycle(ObjectsPoolEntry entry);
    }

    public class ObjectsPool<T> : FastList<ObjectsPoolEntry>, IObjectsPool, IDestroyable
        where T : ObjectsPoolEntry, new()
    {
        public ObjectsPool()
        {
        }

        public T NextObject()
        {
            ObjectsPoolEntry first = RemoveFirstItem();
            if (first == null)
            {
                first = new T();
            }

            first.pool = this;
            return (T)first;
        }

        public void Recycle(ObjectsPoolEntry e)
        {
            Debug.Assert(e is T);
            Debug.Assert(e.pool == this);

            AddLastItem(e);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Destroyable

        public void Destroy()
        {
            Clear();
        }

        #endregion
        
    }

    public class ObjectsPoolEntry : FastListNode
    {
        internal IObjectsPool pool;

        public void Recycle()
        {   
            if (pool != null)
            {
                pool.Recycle(this);
            }
            
            OnRecycleObject();
        }

        protected virtual void OnRecycleObject()
        {
        }
    }
}

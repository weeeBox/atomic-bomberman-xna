using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;

namespace BomberEngine.Core
{
    public delegate void UpdatableDelegate(float delta);

    public class UpdatableDelegateClass : IUpdatable, IDestroyable
    {
        private static UpdatableDelegateClass freeRoot;

        internal UpdatableDelegate updatableDelegate;
        private UpdatableDelegateClass next;

        private UpdatableDelegateClass()
        {
        }

        public static UpdatableDelegateClass Create(UpdatableDelegate updatableDelegate)
        {
            if (updatableDelegate == null)
            {
                throw new ArgumentNullException("Delegate is null");
            }

            UpdatableDelegateClass obj = NextFreeObject();
            obj.updatableDelegate = updatableDelegate;

            return obj;
        }

        //////////////////////////////////////////////////////////////////////////////

        public void Update(float delta)
        {
            updatableDelegate(delta);
        }

        //////////////////////////////////////////////////////////////////////////////

        public void Destroy()
        {
            Debug.Assert(updatableDelegate != null);
            if (updatableDelegate != null)
            {
                AddFreeCall(this);
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Objects pool

        private static UpdatableDelegateClass NextFreeObject()
        {
            if (freeRoot != null)
            {
                UpdatableDelegateClass obj = freeRoot;
                freeRoot = obj.next;
                return obj;
            }

            return new UpdatableDelegateClass();
        }

        private static void AddFreeCall(UpdatableDelegateClass obj)
        {
            obj.Reset();

            if (freeRoot != null)
            {
                obj.next = freeRoot;
            }

            freeRoot = obj;
        }

        private void Reset()
        {
            updatableDelegate = null;
        }

        #endregion
    }
}

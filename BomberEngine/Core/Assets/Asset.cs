using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Assets
{
    public abstract class Asset
    {
        private bool disposed;

        protected abstract void OnDispose();

        public void Dispose()
        {
            if (disposed)
            {
                throw new InvalidOperationException("Resource already disposed");
            }

            OnDispose();
            disposed = true;
        }

        public bool IsDisposed()
        {
            return disposed;
        }
    }
}

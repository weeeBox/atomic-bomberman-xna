using System;

namespace BomberEngine
{
    public abstract class Asset
    {
        private bool disposed;

        protected virtual void OnDispose()
        {
        }

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

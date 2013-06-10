using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core
{
    public delegate void DelayedCallback(DelayedCall call);

    public class DelayedCall
    {
        public static DelayedCall freeRoot;

        public bool cancelled;

        internal DelayedCallback callback;

        public DelayedCall next;
        public DelayedCall prev;

        internal DelayedCallManager manager;

        internal int numRepeats;
        internal int numRepeated;

        internal float timeout;
        internal double fireTime;
        internal double scheduleTime;

        public String name;

        public void Cancel()
        {
            cancelled = true;
            manager.RemoveCall(this);
        }

        internal void Fire()
        {   
            callback(this);

            if (!cancelled)
            {
                ++numRepeated;
                if (numRepeated == numRepeats)
                {
                    Cancel();
                }
                else
                {
                    fireTime = manager.currentTime + timeout;
                }
            }
        }

        public bool IsRepeated
        {
            get { return numRepeats != 1; }
        }

        public float Timeout
        {
            get { return timeout; }
        }

        public float Elapsed
        {
            get { return (float) (manager.currentTime - scheduleTime); }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Objects pool

        internal static DelayedCall NextFreeCall()
        {
            if (freeRoot != null)
            {
                DelayedCall call = freeRoot;
                freeRoot = call.next;
                return call;
            }

            return new DelayedCall();
        }

        internal static void AddFreeCall(DelayedCall call)
        {
            call.Reset();

            if (freeRoot != null)
            {
                call.next = freeRoot;
            }

            freeRoot = call;
        }

        private void Reset()
        {
            next = prev = null;
            manager = null;
            callback = null;
            numRepeats = numRepeated = 0;
            timeout = 0;
            fireTime = 0;
            cancelled = false;
            name = null;
        }

        #endregion
    }
}
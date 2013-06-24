using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;

namespace BomberEngine.Core
{
    public delegate void DelayedCallback(DelayedCall call);

    public class DelayedCall
    {
        public static DelayedCall freeRoot;

        public bool cancelled;

        private const int maxId = 2000000000;
        private static int nextId;
        private int id;

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
            if (manager != null)
            {
                manager.RemoveCall(this);
                manager = null;
            }
        }

        internal void Fire()
        {
            int oldId = id;

            callback(this);

            if (oldId == id) // timer may be cancelled inside callback call and then reused: check if it's the same time (not a reused instance)
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
            DelayedCall call;
            if (freeRoot != null)
            {
                call = freeRoot;
                freeRoot = call.next;
            }
            else
            {
                call = new DelayedCall();
            }

            nextId = nextId == maxId ? 1 : (nextId + 1); // we need non-zero value
            call.id = nextId;

            return call;
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
            id = 0;
            next = prev = null;
            manager = null;
            callback = null;
            numRepeats = numRepeated = 0;
            timeout = 0;
            fireTime = 0;
            scheduleTime = 0;
            cancelled = false;
            name = null;
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;

namespace BomberEngine.Core
{
    public delegate void TimerCallback(Timer call);

    public class Timer
    {
        public static Timer freeRoot;

        public bool cancelled;

        private const int maxId = 2000000000;
        private static int nextId;
        private int id;

        internal TimerCallback callback;

        public Timer next;
        public Timer prev;

        internal TimerManager manager;

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
                manager.RemoveTimer(this);
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

        internal static Timer NextFreeTimer()
        {
            Timer timer;
            if (freeRoot != null)
            {
                timer = freeRoot;
                freeRoot = timer.next;
            }
            else
            {
                timer = new Timer();
            }

            nextId = nextId == maxId ? 1 : (nextId + 1); // we need non-zero value
            timer.id = nextId;

            return timer;
        }

        internal static void AddFreeTimer(Timer timer)
        {
            timer.Reset();

            if (freeRoot != null)
            {
                timer.next = freeRoot;
            }

            freeRoot = timer;
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
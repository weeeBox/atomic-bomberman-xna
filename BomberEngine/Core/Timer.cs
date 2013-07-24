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
        public Object userData;

        public void Cancel()
        {   
            if (!cancelled)
            {
                cancelled = true;
                manager.CancelTimer(this);
                manager = null;
            }
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

        internal static Timer NextFreeTimer()
        {
            Timer timer;
            if (freeRoot != null)
            {
                timer = freeRoot;
                freeRoot = timer.next;
                timer.prev = timer.next = null;
            }
            else
            {
                timer = new Timer();
            }

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
            next = prev = null;
            manager = null;
            callback = null;
            numRepeats = numRepeated = 0;
            timeout = 0;
            fireTime = 0;
            scheduleTime = 0;
            cancelled = false;
            name = null;
            userData = null;
        }

        #endregion
    }
}
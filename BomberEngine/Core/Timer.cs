using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core
{
    public delegate void TimerCallback(Timer timer);

    public class Timer
    {
        public static Timer freeRoot;

        public bool cancelled;

        internal TimerCallback callback;

        public Timer next;
        public Timer prev;

        internal TimerManager timerManager;

        internal int numRepeats;
        internal int numRepeated;

        internal float timeout;
        internal double fireTime;

        public String name;

        public void Cancel()
        {
            cancelled = true;
            timerManager.RemoveTimer(this);
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
                    fireTime = timerManager.currentTime + timeout;
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

        //////////////////////////////////////////////////////////////////////////////

        #region Timers pool

        internal static Timer NextFreeTimer()
        {
            if (freeRoot != null)
            {
                Timer timer = freeRoot;
                freeRoot = timer.next;
                return timer;
            }

            return new Timer();
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
            timerManager = null;
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
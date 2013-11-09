using System;

namespace BomberEngine
{
    public delegate void TimerCallback1();
    public delegate void TimerCallback2(Timer call);

    public class Timer
    {
        internal static Timer freeRoot;

        internal bool cancelled;

        internal TimerCallback1 callback1;
        internal TimerCallback2 callback2;

        internal Timer next;
        internal Timer prev;

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
            callback2(this);

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

        internal static void DefaultTimerCallback(Timer timer)
        {
            timer.callback1();
        }

        public T UserData<T>() where T : class
        {
            return ClassUtils.Cast<T>(userData);
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
            callback1 = null;
            callback2 = null;
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
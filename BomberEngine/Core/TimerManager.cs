using System;

namespace BomberEngine
{
    public class TimerManager : ITimerManager
    {
        public static readonly ITimerManager Null = new NullTimerManager();

        internal double currentTime;

        protected Timer rootTimer;

        protected Timer delayedAddRootTimer; // timers which were scheduled while iterating the list
        protected Timer delayedFreeRootTimer; // timers which were cancelled while iterating the list

        private int timersCount;
        private bool updating;

        public TimerManager()
        {
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Updatable

        public override void Update(float delta)
        {
            currentTime += delta;

            if (timersCount > 0)
            {
                updating = true;
                for (Timer t = rootTimer; t != null; )
                {
                    if (t.fireTime > currentTime)
                    {
                        break;
                    }

                    Timer timer = t;
                    t = t.next;

                    timer.Fire();
                }
                updating = false;
                
                // Put timers which were cancelled during this update back into the pool
                if (delayedFreeRootTimer != null)
                {
                    for (Timer t = delayedFreeRootTimer; t != null; )
                    {
                        Timer timer = t;
                        t = t.next;

                        AddFreeTimer(timer);
                    }
                    delayedFreeRootTimer = null;
                }

                // Add timers which were scheduled during this update
                if (delayedAddRootTimer != null)
                {
                    for (Timer t = delayedAddRootTimer; t != null; )
                    {
                        Timer timer = t;
                        t = t.next;

                        AddTimer(timer);
                    }
                    delayedAddRootTimer = null;
                }
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Schedule

        public override Timer Schedule(TimerCallback1 callback, float delay, int numRepeats, String name = null)
        {
            return Schedule(callback, Timer.DefaultTimerCallback, delay, numRepeats, name);
        }

        public override Timer Schedule(TimerCallback2 callback, float delay, int numRepeats, String name = null)
        {
            return Schedule(null, callback, delay, numRepeats, name);
        }

        private Timer Schedule(TimerCallback1 callback1, TimerCallback2 callback2, float delay, int numRepeats, String name)
        {
            float timeout = delay < 0 ? 0 : delay;

            Timer timer = NextFreeTimer();
            timer.callback1 = callback1;
            timer.callback2 = callback2;
            timer.timeout = timeout;
            timer.numRepeats = numRepeats;
            timer.scheduleTime = currentTime;
            timer.fireTime = currentTime + timeout;
            timer.name = name;

            if (updating)
            {
                AddTimerDelayed(timer);
            }
            else
            {
                AddTimer(timer);
            }

            return timer;
        }

        public override Timer FindTimer(TimerCallback1 callback)
        {
            for (Timer timer = rootTimer; timer != null; timer = timer.next)
            {
                if (timer.callback1 == callback)
                {
                    return timer;
                }
            }

            return null;
        }

        public override Timer FindTimer(TimerCallback2 callback)
        {
            for (Timer timer = rootTimer; timer != null; timer = timer.next)
            {
                if (timer.callback2 == callback)
                {
                    return timer;
                }
            }

            return null;
        }

        public override void Cancel(TimerCallback1 callback)
        {
            for (Timer timer = rootTimer; timer != null; )
            {
                Timer t = timer;
                timer = timer.next;

                if (t.callback1 == callback)
                {
                    t.Cancel();
                }
            }
        }

        public override void Cancel(TimerCallback2 callback)
        {
            for (Timer timer = rootTimer; timer != null;)
            {
                Timer t = timer;
                timer = timer.next;

                if (t.callback2 == callback)
                {
                    t.Cancel();
                }
            }
        }

        public override void Cancel(String name)
        {
            for (Timer timer = rootTimer; timer != null; )
            {
                Timer t = timer;
                timer = timer.next;

                if (t.name == name)
                {
                    t.Cancel();
                }
            }
        }

        public override void CancelAll(Object target)
        {
            for (Timer timer = rootTimer; timer != null; )
            {
                Timer t = timer;
                timer = timer.next;

                if (t.callback1 != null && t.callback1.Target == target || t.callback2.Target == target)
                {
                    t.Cancel();
                }
            }
        }

        public override void CancelAll()
        {
            for (Timer timer = rootTimer; timer != null; )
            {
                Timer t = timer;
                timer = timer.next;

                t.Cancel();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Destroyable

        public override void Destroy()
        {
            CancelAll();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Timer List

        private Timer NextFreeTimer()
        {
            Timer timer = Timer.NextFreeTimer();
            timer.manager = this;
            return timer;
        }

        private void AddTimerDelayed(Timer timer)
        {
            timer.next = null;
            timer.prev = null;

            if (delayedAddRootTimer != null)
            {
                delayedAddRootTimer.next = timer;
            }
            delayedAddRootTimer = timer;
        }

        private void AddFreeTimerDelayed(Timer timer)
        {
            timer.next = null;
            timer.prev = null;

            if (delayedFreeRootTimer != null)
            {
                delayedFreeRootTimer.next = timer;
            }
            delayedFreeRootTimer = timer;
        }

        private void AddFreeTimer(Timer timer)
        {
            Timer.AddFreeTimer(timer);
        }

        private void AddTimer(Timer timer)
        {
            Debug.Assert(timer.manager == this);
            ++timersCount;

            if (rootTimer != null)
            {
                // if timer has the least remaining time - it goes first
                if (timer.fireTime < rootTimer.fireTime)
                {
                    timer.next = rootTimer;
                    rootTimer.prev = timer;
                    rootTimer = timer;

                    return;
                }

                // try to insert in a sorted order
                Timer tail = rootTimer;
                for (Timer t = rootTimer.next; t != null; tail = t, t = t.next)
                {
                    if (timer.fireTime < t.fireTime)
                    {
                        Timer prev = t.prev;
                        Timer next = t;

                        timer.prev = prev;
                        timer.next = next;

                        next.prev = timer;
                        prev.next = timer;

                        return;
                    }
                }

                // add timer at the end of the list
                tail.next = timer;
                timer.prev = tail;
            }
            else
            {
                rootTimer = timer; // timer is root now
            }
        }

        internal void CancelTimer(Timer timer)
        {   
            Debug.Assert(timer.manager == this);
            Debug.Assert(timersCount > 0);
            --timersCount;

            Timer prev = timer.prev;
            Timer next = timer.next;

            if (prev != null) prev.next = next;
            else rootTimer = next;

            if (next != null) next.prev = prev;

            if (updating)
            {
                AddFreeTimerDelayed(timer);
            }
            else
            {
                AddFreeTimer(timer);
            }
            
        }

        public override int Count()
        {
            return timersCount;
        }

        #endregion
    }

    internal class NullTimerManager : ITimerManager
    {
        public override Timer Schedule(TimerCallback1 callback, float delay, int numRepeats, string name = null)
        {
            throw new InvalidOperationException("Can't schedule timer on a 'null' timer manager");
        }

        public override Timer Schedule(TimerCallback2 callback, float delay, int numRepeats, string name = null)
        {
            throw new InvalidOperationException("Can't schedule timer on a 'null' timer manager");
        }

        public override void Cancel(TimerCallback1 callback)
        {   
        }

        public override void Cancel(TimerCallback2 callback)
        {
        }

        public override void Cancel(string name)
        {
        }

        public override void CancelAll()
        {
        }

        public override void CancelAll(object target)
        {
        }

        public override Timer FindTimer(TimerCallback1 callback)
        {
            return null;
        }

        public override Timer FindTimer(TimerCallback2 callback)
        {
            return null;
        }

        public override int Count()
        {
            return 0;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;

namespace BomberEngine.Core
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

        public void Update(float delta)
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

        public Timer Schedule(TimerCallback callback)
        {
            return Schedule(callback, 0.0f, null);
        }

        public Timer Schedule(TimerCallback callback, float delay)
        {
            return Schedule(callback, delay, null);
        }

        public Timer Schedule(TimerCallback callback, float delay, String name)
        {
            return Schedule(callback, delay, false, name);
        }

        public Timer Schedule(TimerCallback callback, float delay, bool repeated)
        {
            return Schedule(callback, delay, repeated, null);
        }

        public Timer Schedule(TimerCallback callback, float delay, bool repeated, String name)
        {
            return Schedule(callback, delay, repeated ? 0 : 1, name);
        }

        public Timer Schedule(TimerCallback callback, float delay, int numRepeats)
        {
            return Schedule(callback, delay, numRepeats, null);
        }

        public Timer ScheduleOnce(TimerCallback callback)
        {
            return ScheduleOnce(callback, 0.0f, null);
        }

        public Timer ScheduleOnce(TimerCallback callback, float delay)
        {
            return ScheduleOnce(callback, delay, null);
        }

        public Timer ScheduleOnce(TimerCallback callback, float delay, String name)
        {
            return ScheduleOnce(callback, delay, false, name);
        }

        public Timer ScheduleOnce(TimerCallback callback, float delay, bool repeated)
        {
            return ScheduleOnce(callback, delay, repeated, null);
        }

        public Timer ScheduleOnce(TimerCallback callback, float delay, bool repeated, String name)
        {
            return ScheduleOnce(callback, delay, repeated ? 0 : 1, name);
        }

        public Timer ScheduleOnce(TimerCallback callback, float delay, int numRepeats)
        {
            return ScheduleOnce(callback, delay, numRepeats, null);
        }

        public Timer ScheduleOnce(TimerCallback callback, float delay, int numRepeats, String name)
        {
            Timer timer = FindTimer(callback);
            if (timer != null)
            {
                return timer;
            }
            
            return Schedule(callback, delay, numRepeats, name);
        }

        public Timer Schedule(TimerCallback callback, float delay, int numRepeats, String name)
        {
            float timeout = delay < 0 ? 0 : delay;

            Timer timer = NextFreeTimer();
            timer.callback = callback;
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

        public Timer FindTimer(TimerCallback callback)
        {
            for (Timer timer = rootTimer; timer != null; timer = timer.next)
            {
                if (timer.callback == callback)
                {
                    return timer;
                }
            }

            return null;
        }

        public bool IsScheduled(TimerCallback callback)
        {
            return FindTimer(callback) != null;
        }

        public void Cancel(TimerCallback callback)
        {
            for (Timer timer = rootTimer; timer != null;)
            {
                Timer t = timer;
                timer = timer.next;

                if (t.callback == callback)
                {
                    t.Cancel();
                }
            }
        }

        public void Cancel(String name)
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

        public void CancelAll(Object target)
        {
            for (Timer timer = rootTimer; timer != null; )
            {
                Timer t = timer;
                timer = timer.next;

                if (t.callback.Target == target)
                {
                    t.Cancel();
                }
            }
        }

        public void CancelAll()
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

        public void Destroy()
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

        public int Count()
        {
            return timersCount;
        }

        #endregion
    }

    internal class NullTimerManager : ITimerManager
    {
        public Timer Schedule(TimerCallback callback)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public Timer Schedule(TimerCallback callback, float delay)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public Timer Schedule(TimerCallback callback, float delay, bool repeated)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public Timer Schedule(TimerCallback callback, float delay, bool repeated, string name)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public Timer Schedule(TimerCallback callback, float delay, int numRepeats)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public Timer Schedule(TimerCallback callback, float delay, int numRepeats, string name)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public Timer Schedule(TimerCallback callback, float delay, string name)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public Timer ScheduleOnce(TimerCallback callback)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public Timer ScheduleOnce(TimerCallback callback, float delay)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public Timer ScheduleOnce(TimerCallback callback, float delay, bool repeated)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public Timer ScheduleOnce(TimerCallback callback, float delay, bool repeated, string name)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public Timer ScheduleOnce(TimerCallback callback, float delay, int numRepeats)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public Timer ScheduleOnce(TimerCallback callback, float delay, int numRepeats, string name)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public Timer ScheduleOnce(TimerCallback callback, float delay, string name)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public void Cancel(TimerCallback callback)
        {
        }

        public void Cancel(String name)
        {
        }

        public void CancelAll()
        {
        }

        public void CancelAll(object target)
        {   
        }

        public Timer FindTimer(TimerCallback callback)
        {
            return null;
        }

        public bool IsScheduled(TimerCallback callback)
        {
            return false;
        }

        public int Count()
        {
            return 0;
        }

        public void Update(float delta)
        {
        }

        public void Destroy()
        {   
        }
    }
}
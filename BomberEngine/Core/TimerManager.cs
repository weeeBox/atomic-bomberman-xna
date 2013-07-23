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

        private int timersCount;

        public TimerManager()
        {
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Updatable

        public void Update(float delta)
        {
            currentTime += delta;
            for (Timer t = rootTimer; t != null;)
            {
                if (t.fireTime > currentTime)
                {
                    break;
                }

                Timer timer = t;
                t = t.next;

                timer.Fire();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Schedule

        public void Schedule(TimerCallback callback)
        {
            Schedule(callback, 0.0f, null);
        }

        public void Schedule(TimerCallback callback, float delay)
        {
            Schedule(callback, delay, null);
        }

        public void Schedule(TimerCallback callback, float delay, String name)
        {
            Schedule(callback, delay, false, name);
        }

        public void Schedule(TimerCallback callback, float delay, bool repeated)
        {
            Schedule(callback, delay, repeated, null);
        }

        public void Schedule(TimerCallback callback, float delay, bool repeated, String name)
        {
            Schedule(callback, delay, repeated ? 0 : 1, name);
        }

        public void Schedule(TimerCallback callback, float delay, int numRepeats)
        {
            Schedule(callback, delay, numRepeats, null);
        }

        public void ScheduleOnce(TimerCallback callback)
        {
            ScheduleOnce(callback, 0.0f, null);
        }

        public void ScheduleOnce(TimerCallback callback, float delay)
        {
            ScheduleOnce(callback, delay, null);
        }

        public void ScheduleOnce(TimerCallback callback, float delay, String name)
        {
            ScheduleOnce(callback, delay, false, name);
        }

        public void ScheduleOnce(TimerCallback callback, float delay, bool repeated)
        {
            ScheduleOnce(callback, delay, repeated, null);
        }

        public void ScheduleOnce(TimerCallback callback, float delay, bool repeated, String name)
        {
            ScheduleOnce(callback, delay, repeated ? 0 : 1, name);
        }

        public void ScheduleOnce(TimerCallback callback, float delay, int numRepeats)
        {
            ScheduleOnce(callback, delay, numRepeats, null);
        }

        public void ScheduleOnce(TimerCallback callback, float delay, int numRepeats, String name)
        {
            if (!IsScheduled(callback))
            {
                ScheduleOnce(callback, delay, numRepeats, name);
            }
        }

        public void Schedule(TimerCallback callback, float delay, int numRepeats, String name)
        {
            float timeout = delay < 0 ? 0 : delay;

            Timer timer = NextFreeTimer();
            timer.callback = callback;
            timer.timeout = timeout;
            timer.numRepeats = numRepeats;
            timer.scheduleTime = currentTime;
            timer.fireTime = currentTime + timeout;
            timer.name = name;

            AddTimer(timer);
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

        internal void RemoveTimer(Timer timer)
        {
            Debug.Assert(timer.manager == this);
            Debug.Assert(timersCount > 0);
            --timersCount;

            Timer prev = timer.prev;
            Timer next = timer.next;

            if (prev != null) prev.next = next;
            else rootTimer = next;

            if (next != null) next.prev = prev;

            AddFreeTimer(timer);
        }

        public int Count()
        {
            return timersCount;
        }

        #endregion
    }

    internal class NullTimerManager : ITimerManager
    {
        public void Schedule(TimerCallback callback)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public void Schedule(TimerCallback callback, float delay)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public void Schedule(TimerCallback callback, float delay, bool repeated)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public void Schedule(TimerCallback callback, float delay, bool repeated, string name)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public void Schedule(TimerCallback callback, float delay, int numRepeats)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public void Schedule(TimerCallback callback, float delay, int numRepeats, string name)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public void Schedule(TimerCallback callback, float delay, string name)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public void ScheduleOnce(TimerCallback callback)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public void ScheduleOnce(TimerCallback callback, float delay)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public void ScheduleOnce(TimerCallback callback, float delay, bool repeated)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public void ScheduleOnce(TimerCallback callback, float delay, bool repeated, string name)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public void ScheduleOnce(TimerCallback callback, float delay, int numRepeats)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public void ScheduleOnce(TimerCallback callback, float delay, int numRepeats, string name)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public void ScheduleOnce(TimerCallback callback, float delay, string name)
        {
            throw new InvalidOperationException("Can't schedule timer on 'null' timer manager");
        }

        public void Cancel(TimerCallback callback)
        {
        }

        public void Cancel(string name)
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
using System;
namespace BomberEngine.Core
{
    public abstract class ITimerManager : IUpdatable, IDestroyable // not a good idea to name abstract class as interface, but what you gonna do
    {
        public virtual void Update(float delta)
        {
        }

        public virtual void Destroy()
        {
        }

        public Timer Schedule(TimerCallback1 callback, float delay = 0.0f, bool repeated = false, string name = null)
        {
            return Schedule(callback, delay, repeated ? 0 : 1, name);
        }

        public Timer Schedule(TimerCallback2 callback, float delay = 0.0f, bool repeated = false, string name = null)
        {
            return Schedule(callback, delay, repeated ? 0 : 1, name);
        }

        public Timer ScheduleOnce(TimerCallback1 callback, float delay = 0.0f, bool repeated = false, string name = null)
        {
            return ScheduleOnce(callback, delay, repeated ? 0 : 1, name);
        }

        public Timer ScheduleOnce(TimerCallback2 callback, float delay = 0.0f, bool repeated = false, string name = null)
        {
            return ScheduleOnce(callback, delay, repeated ? 0 : 1, name);
        }

        public Timer ScheduleOnce(TimerCallback1 callback, float delay, int numRepeats, string name = null)
        {
            Timer timer = FindTimer(callback);
            if (timer != null)
            {
                return timer;
            }

            return Schedule(callback, delay, numRepeats, name);
        }

        public Timer ScheduleOnce(TimerCallback2 callback, float delay, int numRepeats, string name = null)
        {
            Timer timer = FindTimer(callback);
            if (timer != null)
            {
                return timer;
            }

            return Schedule(callback, delay, numRepeats, name);
        }

        public bool IsScheduled(TimerCallback1 callback)
        {
            return FindTimer(callback) != null;
        }

        public bool IsScheduled(TimerCallback2 callback)
        {
            return FindTimer(callback) != null;
        }

        public abstract Timer Schedule(TimerCallback1 callback, float delay, int numRepeats, string name = null);
        public abstract Timer Schedule(TimerCallback2 callback, float delay, int numRepeats, string name = null);

        public abstract void Cancel(TimerCallback1 callback);
        public abstract void Cancel(TimerCallback2 callback);

        public abstract void Cancel(string name);
        public abstract void CancelAll();
        public abstract void CancelAll(object target);

        public abstract Timer FindTimer(TimerCallback1 callback);
        public abstract Timer FindTimer(TimerCallback2 callback);

        public abstract int Count();
    }
}

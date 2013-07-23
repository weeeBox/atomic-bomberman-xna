using System;
namespace BomberEngine.Core
{
    public interface ITimerManager : IUpdatable, IDestroyable
    {   
        void Schedule(TimerCallback callback);
        void Schedule(TimerCallback callback, float delay);
        void Schedule(TimerCallback callback, float delay, bool repeated);
        void Schedule(TimerCallback callback, float delay, bool repeated, string name);
        void Schedule(TimerCallback callback, float delay, int numRepeats);
        void Schedule(TimerCallback callback, float delay, int numRepeats, string name);
        void Schedule(TimerCallback callback, float delay, string name);
        void ScheduleOnce(TimerCallback callback);
        void ScheduleOnce(TimerCallback callback, float delay);
        void ScheduleOnce(TimerCallback callback, float delay, bool repeated);
        void ScheduleOnce(TimerCallback callback, float delay, bool repeated, string name);
        void ScheduleOnce(TimerCallback callback, float delay, int numRepeats);
        void ScheduleOnce(TimerCallback callback, float delay, int numRepeats, string name);
        void ScheduleOnce(TimerCallback callback, float delay, string name);

        void Cancel(TimerCallback callback);
        void Cancel(string name);
        void CancelAll();
        void CancelAll(object target);

        Timer FindTimer(TimerCallback callback);
        bool IsScheduled(TimerCallback callback);

        int Count();
    }
}

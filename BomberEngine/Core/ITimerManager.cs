using System;
namespace BomberEngine.Core
{
    public interface ITimerManager : IUpdatable, IDestroyable
    {   
        Timer Schedule(TimerCallback callback);
        Timer Schedule(TimerCallback callback, float delay);
        Timer Schedule(TimerCallback callback, float delay, bool repeated);
        Timer Schedule(TimerCallback callback, float delay, bool repeated, string name);
        Timer Schedule(TimerCallback callback, float delay, int numRepeats);
        Timer Schedule(TimerCallback callback, float delay, int numRepeats, string name);
        Timer Schedule(TimerCallback callback, float delay, string name);
        Timer ScheduleOnce(TimerCallback callback);
        Timer ScheduleOnce(TimerCallback callback, float delay);
        Timer ScheduleOnce(TimerCallback callback, float delay, bool repeated);
        Timer ScheduleOnce(TimerCallback callback, float delay, bool repeated, string name);
        Timer ScheduleOnce(TimerCallback callback, float delay, int numRepeats);
        Timer ScheduleOnce(TimerCallback callback, float delay, int numRepeats, string name);
        Timer ScheduleOnce(TimerCallback callback, float delay, string name);

        void Cancel(TimerCallback callback);
        void Cancel(string name);
        void CancelAll();
        void CancelAll(object target);

        Timer FindTimer(TimerCallback callback);
        bool IsScheduled(TimerCallback callback);

        int Count();
    }
}

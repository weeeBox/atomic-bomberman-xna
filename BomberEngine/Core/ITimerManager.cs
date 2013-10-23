using System;
namespace BomberEngine.Core
{
    public interface ITimerManager : IUpdatable, IDestroyable
    {   
        Timer Schedule(TimerCallback2 callback);
        Timer Schedule(TimerCallback2 callback, float delay);
        Timer Schedule(TimerCallback2 callback, float delay, bool repeated);
        Timer Schedule(TimerCallback2 callback, float delay, bool repeated, string name);
        Timer Schedule(TimerCallback2 callback, float delay, int numRepeats);
        Timer Schedule(TimerCallback2 callback, float delay, int numRepeats, string name);
        Timer Schedule(TimerCallback2 callback, float delay, string name);
        Timer ScheduleOnce(TimerCallback2 callback);
        Timer ScheduleOnce(TimerCallback2 callback, float delay);
        Timer ScheduleOnce(TimerCallback2 callback, float delay, bool repeated);
        Timer ScheduleOnce(TimerCallback2 callback, float delay, bool repeated, string name);
        Timer ScheduleOnce(TimerCallback2 callback, float delay, int numRepeats);
        Timer ScheduleOnce(TimerCallback2 callback, float delay, int numRepeats, string name);
        Timer ScheduleOnce(TimerCallback2 callback, float delay, string name);

        void Cancel(TimerCallback2 callback);
        void Cancel(string name);
        void CancelAll();
        void CancelAll(object target);

        Timer FindTimer(TimerCallback2 callback);
        bool IsScheduled(TimerCallback2 callback);

        int Count();
    }
}

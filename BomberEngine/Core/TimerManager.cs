using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core
{
    public class TimerManager : Updatable
    {   
        private List<Timer> timers;
        
        public TimerManager()
        {
            timers = new List<Timer>();
        }

        public void Update(float delta)
        {
            int timerIndex = 0;
            int timersCount = timers.Count;

            while (timerIndex < timersCount)
            {
                Timer timer = timers[timerIndex];

                if (timer.IsCancelled)
                {
                    timers.RemoveAt(timerIndex);
                    --timersCount;
                    continue;
                }

                timer.AdvanceTime(delta);

                if (timer.IsCancelled)
                {
                    timers.RemoveAt(timerIndex);
                    --timersCount;
                    continue;
                }

                ++timerIndex;
            }
        }

        public Timer Schedule(TimerCallback callback, float delay)
        {
            return Schedule(callback, delay, false);
        }

        public Timer Schedule(TimerCallback callback, float delay, Boolean repeated)
        {
            return ScheduleRepeated(callback, delay, repeated ? 0 : 1);
        }

        public Timer ScheduleRepeated(TimerCallback callback, float delay, int numRepeats)
        {
            Timer timer = new Timer(callback, numRepeats);
            timers.Add(timer);
            return timer;
        }

        public int TimersCount
        {
            get { return timers.Count; }
        }

        public int AliveTimersCount
        {
            get
            {
                int aliveCount = 0;
                foreach (Timer timer in timers)
                {
                    if (!timer.IsCancelled)
                    {
                        ++aliveCount;
                    }
                }
                return aliveCount;
            }
        }

    }
}

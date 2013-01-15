using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core
{
    public class TimerManager : Updatable
    {   
        // TODO: reuse objects + store timers in a sorted order (for a faster iteration)
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
            return Schedule(callback, delay, repeated ? 0 : 1);
        }

        public Timer Schedule(TimerCallback callback, float delay, int numRepeats)
        {
            Timer timer = new Timer(callback, delay, numRepeats);
            AddTimer(timer);
            return timer;
        }

        private void AddTimer(Timer timer)
        {
            timers.Add(timer);
        }

        public int GetTimersCount()
        {
            return timers.Count;
        }
    }
}
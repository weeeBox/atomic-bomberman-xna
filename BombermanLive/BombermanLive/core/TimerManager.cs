using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
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
            while (timerIndex < timers.Count)
            {
                Timer timer = timers[timerIndex];

                if (timer.IsCancelled)
                {
                    timers.RemoveAt(timerIndex);
                    continue;
                }

                timer.AdvanceTime(delta);

                if (timer.RemainingTime == 0)
                {
                    timer.Fire();
                    if (timer.IsDoneRepeating)
                    {
                        timers.RemoveAt(timerIndex);
                        continue;
                    }
                }

                ++timerIndex;
            }
        }
    }
}

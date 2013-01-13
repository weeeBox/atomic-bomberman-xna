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

        private TimerManagerListener listener;

        public TimerManager()
            : this(null)
        {
        }

        public TimerManager(TimerManagerListener listener)
        {   
            this.listener = listener;
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
                    NotifyTimerRemoved(timer);
                    --timersCount;
                    continue;
                }

                timer.AdvanceTime(delta);

                if (timer.IsCancelled)
                {
                    timers.RemoveAt(timerIndex);
                    NotifyTimerRemoved(timer);
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
            NotifyTimerAdded(timer);
        }

        public int TimersCount
        {
            get { return timers.Count; }
        }

        public TimerManagerListener Listener
        {
            get { return listener; }
            set { listener = value; }
        }

        private void NotifyTimerAdded(Timer timer)
        {
            if (listener != null)
            {
                listener.OnTimerAdded(this, timer);
            }
        }

        private void NotifyTimerRemoved(Timer timer)
        {
            if (listener != null)
            {
                listener.OnTimerRemoved(this, timer);
            }
        }
    }
}
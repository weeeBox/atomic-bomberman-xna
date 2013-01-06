using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core
{
    public delegate void TimerCallback(Timer timer);

    public class Timer
    {
        private TimerCallback callback;

        private int numRepeats;
        private int numRepeated;

        private float elaspedTime;
        private float timeout;

        private bool cancelled;

        public Timer(TimerCallback callback, float timeout) : this(callback, timeout, 1)
        {
        }

        public Timer(TimerCallback callback, float timeout, int numRepeats)
        {
            if (callback == null)
            {
                throw new ArgumentException("Callback cannot be null");
            }

            if (numRepeats < 0)
            {
                throw new ArgumentException("Illegal number of repeats: " + numRepeats);
            }

            this.callback = callback;
            this.timeout = timeout < 0 ? 0 : timeout;
            this.numRepeats = numRepeats;
        }

        public void Cancel()
        {
            cancelled = true;
        }

        public void Reset()
        {   
            numRepeated = 0;
            elaspedTime = 0;
        }

        public void AdvanceTime(float delta)
        {
            elaspedTime += delta;
            if (elaspedTime >= timeout)
            {
                Fire();
            }
        }

        protected void Fire()
        {   
            callback(this);

            ++numRepeated;
            if (numRepeated == numRepeats)
            {
                Cancel();
            }
            else
            {
                Reset();
            }
        }

        public bool IsRepeated
        {
            get { return numRepeats != 1; }
        }

        public float ElaspedTime
        {
            get { return elaspedTime; }
        }

        public float RemainingTime
        {
            get { return Math.Max(0, timeout - elaspedTime); }
        }

        public float Timeout
        {
            get { return timeout; }
        }

        public bool IsCancelled
        {
            get { return cancelled; }
        }
    }
}
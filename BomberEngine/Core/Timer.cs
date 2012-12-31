using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core
{
    public abstract class Timer
    {
        private int numRepeats;
        private int numRepeated;

        private float elaspedTime;
        private float timeout;

        private bool cancelled;

        protected abstract void onTimer(Timer timer);

        public void Cancel()
        {
            cancelled = true;
        }

        internal void AdvanceTime(float delta)
        {
            elaspedTime += delta;
        }

        internal void Fire()
        {
            onTimer(this);
            ++numRepeated;
        }

        internal bool IsDoneRepeating
        {
            get { return numRepeats != 0 && numRepeats == numRepeated; }
        }

        public bool Repeated
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

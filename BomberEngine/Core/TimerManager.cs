using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;

namespace BomberEngine.Core
{
    public class TimerManager : IUpdatable, IDestroyable
    {
        internal double currentTime;

        protected Timer rootCall;

        private int callsCount;

        public TimerManager()
        {   
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Updatable

        public void Update(float delta)
        {
            currentTime += delta;
            for (Timer c = rootCall; c != null;)
            {
                if (c.fireTime > currentTime)
                {
                    break;
                }

                Timer call = c;
                c = c.next;

                call.Fire();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Schedule

        public void Schedule(TimerCallback callback)
        {
            Schedule(callback, 0.0f, null);
        }

        public void Schedule(TimerCallback callback, float delay)
        {
            Schedule(callback, delay, null);
        }

        public void Schedule(TimerCallback callback, float delay, String name)
        {
            Schedule(callback, delay, false, name);
        }

        public void Schedule(TimerCallback callback, float delay, bool repeated)
        {
            Schedule(callback, delay, repeated, null);
        }

        public void Schedule(TimerCallback callback, float delay, bool repeated, String name)
        {
            Schedule(callback, delay, repeated ? 0 : 1, name);
        }

        public void Schedule(TimerCallback callback, float delay, int numRepeats)
        {
            Schedule(callback, delay, numRepeats, null);
        }

        public void Schedule(TimerCallback callback, float delay, int numRepeats, String name)
        {
            float timeout = delay < 0 ? 0 : delay;

            Timer call = NextFreeCall();
            call.callback = callback;
            call.timeout = timeout;
            call.numRepeats = numRepeats;
            call.scheduleTime = currentTime;
            call.fireTime = currentTime + timeout;
            call.name = name;

            AddCall(call);
        }

        public void Cancel(TimerCallback callback)
        {
            for (Timer call = rootCall; call != null;)
            {
                Timer c = call;
                call = call.next;

                if (c.callback == callback)
                {
                    c.Cancel();
                }
            }
        }

        public void Cancel(String name)
        {
            for (Timer call = rootCall; call != null; )
            {
                Timer c = call;
                call = call.next;

                if (c.name == name)
                {
                    c.Cancel();
                }
            }
        }

        public void CancelAll(Object target)
        {
            for (Timer call = rootCall; call != null; )
            {
                Timer c = call;
                call = call.next;

                if (c.callback.Target == target)
                {
                    c.Cancel();
                }
            }
        }

        public void CancelAll()
        {
            for (Timer call = rootCall; call != null; )
            {
                Timer c = call;
                call = call.next;

                c.Cancel();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Destroyable

        public void Destroy()
        {
            CancelAll();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Call List

        private Timer NextFreeCall()
        {
            Timer call = Timer.NextFreeTimer();
            call.manager = this;
            return call;
        }

        private void AddFreeCall(Timer call)
        {
            Timer.AddFreeTimer(call);
        }

        private void AddCall(Timer call)
        {
            Debug.Assert(call.manager == this);
            ++callsCount;

            if (rootCall != null)
            {
                // if timer has the least remaining time - it goes first
                if (call.fireTime < rootCall.fireTime)
                {
                    call.next = rootCall;
                    rootCall.prev = call;
                    rootCall = call;

                    return;
                }

                // try to insert in a sorted order
                Timer tail = rootCall;
                for (Timer c = rootCall.next; c != null; tail = c, c = c.next)
                {
                    if (call.fireTime < c.fireTime)
                    {
                        Timer prev = c.prev;
                        Timer next = c;

                        call.prev = prev;
                        call.next = next;

                        next.prev = call;
                        prev.next = call;

                        return;
                    }
                }

                // add timer at the end of the list
                tail.next = call;
                call.prev = tail;
            }
            else
            {
                rootCall = call; // timer is root now
            }
        }

        internal void RemoveTimer(Timer call)
        {
            Debug.Assert(call.manager == this);
            Debug.Assert(callsCount > 0);
            --callsCount;

            Timer prev = call.prev;
            Timer next = call.next;

            if (prev != null) prev.next = next;
            else rootCall = next;

            if (next != null) next.prev = prev;

            AddFreeCall(call);
        }

        public int Count()
        {
            return callsCount;
        }

        #endregion
    }
}
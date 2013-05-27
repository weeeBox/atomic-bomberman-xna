using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;

namespace BomberEngine.Core
{
    public class DelayedCallManager : IUpdatable
    {
        internal double currentTime;

        protected DelayedCall rootCall;

        private int callsCount;

        public DelayedCallManager()
        {   
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Updatable

        public void Update(float delta)
        {
            currentTime += delta;
            for (DelayedCall c = rootCall; c != null;)
            {
                if (c.fireTime > currentTime)
                {
                    break;
                }

                DelayedCall call = c;
                c = c.next;

                call.Fire();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Schedule

        public void Schedule(DelayedCallback callback, float delay)
        {
            Schedule(callback, delay, null);
        }

        public void Schedule(DelayedCallback callback, float delay, String name)
        {
            Schedule(callback, delay, false, name);
        }

        public void Schedule(DelayedCallback callback, float delay, bool repeated)
        {
            Schedule(callback, delay, repeated, null);
        }

        public void Schedule(DelayedCallback callback, float delay, bool repeated, String name)
        {
            Schedule(callback, delay, repeated ? 0 : 1, name);
        }

        public void Schedule(DelayedCallback callback, float delay, int numRepeats)
        {
            Schedule(callback, delay, numRepeats, null);
        }

        public void Schedule(DelayedCallback callback, float delay, int numRepeats, String name)
        {
            float timeout = delay < 0 ? 0 : delay;

            DelayedCall call = NextFreeCall();
            call.callback = callback;
            call.timeout = timeout;
            call.numRepeats = numRepeats;
            call.fireTime = currentTime + timeout;
            call.name = name;

            AddCall(call);
        }

        public void Cancel(DelayedCallback callback)
        {
            for (DelayedCall call = rootCall; call != null;)
            {
                DelayedCall c = call;
                call = call.next;

                if (c.callback == callback)
                {
                    RemoveCall(c);
                }
            }
        }

        public void Cancel(String name)
        {
            for (DelayedCall call = rootCall; call != null; )
            {
                DelayedCall c = call;
                call = call.next;

                if (c.name == name)
                {
                    RemoveCall(c);
                }
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Call List

        private DelayedCall NextFreeCall()
        {
            DelayedCall call = DelayedCall.NextFreeCall();
            call.manager = this;
            return call;
        }

        private void AddFreeCall(DelayedCall call)
        {
            DelayedCall.AddFreeCall(call);
        }

        private void AddCall(DelayedCall call)
        {
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
                DelayedCall tail = rootCall;
                for (DelayedCall c = rootCall.next; c != null; tail = c, c = c.next)
                {
                    if (call.fireTime < c.fireTime)
                    {
                        DelayedCall prev = c.prev;
                        DelayedCall next = c;

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

        internal void RemoveCall(DelayedCall call)
        {
            Debug.Assert(callsCount > 0);
            --callsCount;

            DelayedCall prev = call.prev;
            DelayedCall next = call.next;

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
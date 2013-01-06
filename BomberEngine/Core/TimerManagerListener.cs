using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core
{
    public interface TimerManagerListener
    {
        void OnTimerAdded(TimerManager manager, Timer timer);
        void OnTimerRemoved(TimerManager manager, Timer timer);
    }
}

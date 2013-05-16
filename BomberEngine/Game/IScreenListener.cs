using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Game
{
    public interface IScreenListener
    {
        void OnScreenStarted(Screen screen);

        void OnScreenSuspended(Screen screen);

        void OnScreenResumed(Screen screen);

        void OnScreenStoped(Screen screen);
    }
}

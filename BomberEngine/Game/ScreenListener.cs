using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Game
{
    public interface ScreenListener
    {
        void OnSceneStarted(Screen scene);

        void OnSceneSuspended(Screen scene);

        void OnSceneResumed(Screen scene);

        void OnSceneStoped(Screen scene);
    }
}

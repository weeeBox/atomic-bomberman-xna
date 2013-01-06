using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Game
{
    public interface SceneListener
    {
        void OnSceneStarted(Scene scene);

        void OnSceneSuspended(Scene scene);

        void OnSceneResumed(Scene scene);

        void OnSceneStoped(Scene scene);
    }
}

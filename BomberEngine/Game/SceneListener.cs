using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Game
{
    interface SceneListener
    {
        void OnSceneStarted(Scene scene);
        void OnSceneStoped(Scene scene);
    }
}

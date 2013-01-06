using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;

namespace BomberEngine.Game
{
    public class GameScene : Updatable, Drawable
    {
        private TimerManager timerManager;

        protected void ScheduleTimer(TimerCallback callback)
        {
            if (timerManager == null)
            {
                timerManager = new TimerManager();
            }
        }

        public void Update(float delta)
        {
            
        }

        public void Draw(Context context)
        {
            
        }
    }
}

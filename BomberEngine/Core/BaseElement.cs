using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Events;

namespace BomberEngine.Core
{
    public abstract class BaseElement : BaseObject, IUpdatable, IDrawable, IEventHandler
    {
        public int id;

        public virtual void Update(float delta)
        {   
        }

        public virtual void Draw(Context context)
        {
        }

        public virtual bool HandleEvent(Event evt)
        {
            return false;
        }
    }
}

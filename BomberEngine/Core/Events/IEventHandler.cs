using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Events
{
    public interface IEventHandler
    {
        bool HandleEvent(Event evt);
    }
}

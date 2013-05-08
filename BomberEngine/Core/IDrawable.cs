using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Visual;

namespace BomberEngine.Core
{
    public interface IDrawable
    {
        void Draw(Context context);
    }
}

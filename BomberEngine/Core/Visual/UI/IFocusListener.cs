using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Visual.UI
{
    public interface IFocusListener
    {
        void OnFocusChange(View view, bool gainFocus);
    }
}

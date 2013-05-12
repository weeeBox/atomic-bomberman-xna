using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Visual.UI
{
    public class View : VisualElement
    {
        public int id;

        public bool visible;
        public bool focused;
        public bool enabled;

        public bool focusable;

        public IFocusListener focusListener;

        public View()
        {
        }

        public View(int width, int height)
            : this(0, 0, width, height)
        {
        }

        public View(float x, float y, int width, int height)
            : base(x, y, width, height)
        {
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Focus

        public void requestFocus()
        {

        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Pointer actions

        #endregion
    }
}

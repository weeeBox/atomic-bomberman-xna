using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;

namespace BomberEngine.Core.Visual.UI
{
    public class View : VisualElement
    {
        public enum FocusDirection
        {
            None, Up, Down, Left, Right
        }

        public int id;

        public bool visible;
        public bool focused;
        public bool enabled;

        public bool focusable;

        public IFocusListener focusListener;

        public View()
            : this(0, 0)
        {
        }

        public View(int width, int height)
            : this(0, 0, width, height)
        {
        }

        public View(float x, float y, int width, int height)
            : base(x, y, width, height)
        {
            visible = enabled = true;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Focus

        public virtual bool CanFocus()
        {
            return focusable && visible && enabled;
        }

        public void focus()
        {
            focused = true;
        }

        public void blur()
        {
            focused = false;
        }

        public virtual View FindFocus(FocusDirection direction, View currentFocus)
        {
            switch (direction)
            {
                case FocusDirection.Down:
                case FocusDirection.Right:
                {
                    int startIndex = currentFocus != null ? IndexOf(currentFocus) + 1 : 0;

                    for (int i = startIndex; i < ChildCount(); ++i)
                    {
                        View view = ViewAt(i);
                        Debug.Assert(view != null);

                        View childFocus = view.FindFocus(direction, null);
                        if (childFocus != null)
                        {
                            return childFocus;
                        }

                        if (view.CanFocus())
                        {
                            return view;
                        }
                    }
                    break;
                }   

                default:
                    throw new NotImplementedException();
            }

            return null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Pointer actions

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region View hierarchy        

        public View GetParentView()
        {
            return GetParent() as View;
        }

        public View ViewAt(int index)
        {
            return ChildAt(index) as View;
        }

        #endregion
    }
}

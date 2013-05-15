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
            bool oldFocused = focused;
            focused = true;
            if (focused ^ oldFocused)
            {
                OnFocusChanged(focused);
            }
        }

        public void blur()
        {
            bool oldFocused = focused;
            focused = false;
            if (focused ^ oldFocused)
            {
                OnFocusChanged(focused);
            }
        }

        protected virtual void OnFocusChanged(bool focused)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Pointer actions

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Resize

        public void ResizeToFitViewsHor()
        {
            ResizeToFitViewsHor(0.0f, 0.0f);
        }

        public void ResizeToFitViewsHor(float border)
        {
            ResizeToFitViewsHor(border, border);
        }

        public void ResizeToFitViewsHor(float leftBorder, float rightBorder)
        {
            ResizeToFitViews(true, false, leftBorder, 0.0f, rightBorder, 0.0f);
        }

        public void ResizeToFitViewsVer()
        {
            ResizeToFitViewsVer(0.0f, 0.0f);
        }

        public void ResizeToFitViewsVer(float border)
        {
            ResizeToFitViewsVer(border, border);
        }

        public void ResizeToFitViewsVer(float topBorder, float bottomBorder)
        {
            ResizeToFitViews(false, true, 0.0f, topBorder, 0.0f, bottomBorder);
        }

        public void ResizeToFitViews(bool horizontal, bool vertical, float border)
        {
            ResizeToFitViews(horizontal, vertical, border, border, border, border);
        }

        public void ResizeToFitViews(bool horizontal, bool vertical, float horBorder, float verBorder)
        {
            ResizeToFitViews(horizontal, vertical, horBorder, verBorder, horBorder, verBorder);
        }

        public void ResizeToFitViews(bool horizontal, bool vertical, float leftBorder, float topBorder, float rightBorder, float bottomBorder)
        {
            Debug.Assert(horizontal | vertical);

            if (ChildCount() > 0)
            {
                VisualElement first = ChildAt(0);
                float left = first.x;
                float right = left + first.width;
                float top = first.y;
                float bottom = top + first.height;

                for (int i = 1; i < ChildCount(); ++i)
                {
                    VisualElement e = ChildAt(i);
                    left = Math.Min(left, e.x);
                    right = Math.Max(right, e.x + e.width);
                    top = Math.Min(top, e.y);
                    bottom = Math.Max(bottom, e.y + e.height);
                }

                for (int i = 0; i < ChildCount(); ++i)
                {
                    VisualElement e = ChildAt(i);
                    e.x = horizontal ? leftBorder + e.x - left : e.x;
                    e.y = vertical ? topBorder + e.y - top : e.y;
                }

                width = horizontal ? leftBorder + right - left + rightBorder : width;
                height = vertical ? topBorder + bottom - top + bottomBorder: height;
            }
            else
            {
                width = leftBorder + rightBorder;
                height = topBorder + bottomBorder;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Layout

        public void LayoutHor(float indent)
        {
            LayoutHor(indent, ALIGN_CENTER);
        }

        public void LayoutHor(float indent, float align)
        {   
            float total = 0;
            for (int i = 0; i < ChildCount(); ++i)
            {
                total += ChildAt(i).width + (i < ChildCount() - 1 ? indent : 0);
            }

            float pos = 0.5f * align * (width - total);
            for (int i = 0; i < ChildCount(); ++i)
            {
                VisualElement e = ChildAt(i);
                e.x = pos;
                pos += e.width + indent;
            }
        }

        public void LayoutVer(float indent)
        {
            LayoutVer(indent, ALIGN_MIN);
        }

        public void LayoutVer(float indent, float align)
        {
            float total = 0;
            for (int i = 0; i < ChildCount(); ++i)
            {
                total += ChildAt(i).height+ (i < ChildCount() - 1 ? indent : 0);
            }

            float pos = 0.5f * align * (height - total);
            for (int i = 0; i < ChildCount(); ++i)
            {
                VisualElement e = ChildAt(i);
                e.y = pos;
                pos += e.height + indent;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region View hierarchy        

        public View GetParentView()
        {
            return GetParent() as View;
        }

        public View ChildViewAt(int index)
        {
            return ChildAt(index) as View;
        }

        #endregion
    }
}

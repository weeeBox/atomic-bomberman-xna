using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BomberEngine.Debugging;

namespace BomberEngine.Core.Visual
{
    public class View : BaseElement
    {
        public const float ALIGN_MIN = 0.0f;
        public const float ALIGN_CENTER = 0.5f;
        public const float ALIGN_MAX = 1.0f;

        public bool visible;
        public bool enabled;

        public bool focused;
        public bool focusable;

        public float x;
        public float y;
        
        public float width;
        public float height;

        public float rotation;
        public float rotationCenterX;
        public float rotationCenterY;

        public float scaleX;
        public float scaleY;

        public Color color;

        protected float translateX;
        protected float translateY;

        public float alignX;
        public float alignY;

        public float parentAlignX;
        public float parentAlignY;

        protected View parent;
        protected ViewList viewList;

        public View()
            : this(0, 0)
        {
        }

        public View(int width, int height)
            : this(0, 0, width, height)
        {
        }

        public View(float x, float y, int width, int height)
        {
            this.x = x;
            this.y = y;

            this.width = width;
            this.height = height;

            rotation = 0;
            rotationCenterX = 0;
            rotationCenterY = 0;
            scaleX = 1.0f;
            scaleY = 1.0f;
            color = Color.White; //solidOpaqueRGBA;
            translateX = 0;
            translateY = 0;

            parentAlignX = parentAlignY = alignX = alignY = ALIGN_MIN;
            parent = null;

            viewList = ViewList.Null;

            visible = enabled = true;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Update

        public override void Update(float delta)
        {
            viewList.Update(delta);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Draw

        public override void Draw(Context context)
        {
            PreDraw(context);
            PostDraw(context);
        }

        protected virtual void DrawChildren(Context context)
        {
            viewList.Draw(context);
        }

        protected virtual void PreDraw(Context context)
        {
            // align to parent
            translateX = x - width * alignX;
            translateY = y - height * alignY;

            bool changeScale = (scaleX != 1.0 || scaleY != 1.0);
            bool changeRotation = (rotation != 0.0);
            bool changeTranslate = (translateX != 0.0 || translateY != 0.0);

            // apply transformations
            if (changeTranslate || changeRotation || changeScale)
            {
                context.PushMatrix();

                if (changeRotation || changeScale)
                {
                    float rotationOffsetX = translateX + (0.5f * width) + rotationCenterX;
                    float rotationOffsetY = translateY + (0.5f * height) + rotationCenterY;

                    context.Translate(rotationOffsetX, rotationOffsetY);

                    if (changeRotation)
                    {
                        context.Rotate(rotation, 0, 0, 1);
                    }

                    if (changeScale)
                    {
                        context.Scale(scaleX, scaleY, 1);
                    }
                    context.Translate(-rotationOffsetX, -rotationOffsetY);
                }

                if (changeTranslate)
                {
                    context.Translate(translateX, translateY);
                }
            }

            if (color != Color.White)
            {
                context.SetColor(color);
            }
        }

        protected virtual void PostDraw(Context context)
        {
            DrawChildren(context);
            RestoreTransformations(context);
        }

        protected void RestoreTransformations(Context context)
        {
            if (color != Color.White)
            {
                context.SetColor(Color.White);
            }

            // if any transformation
            if (rotation != 0.0 || scaleX != 1.0 || scaleY != 1.0 || translateX != 0.0 || translateY != 0.0)
            {
                context.PopMatrix();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Hierarchy

        public void AddView(View child)
        {
            if (viewList.IsNull())
            {
                viewList = new ViewList();
            }

            if (child.parent != null)
            {
                parent.RemoveView(child);
            }

            viewList.Add(child);
            child.parent = this;
        }

        public void RemoveView(View child)
        {
            if (viewList.Count() > 0)
            {
                viewList.Remove(child);
                child.parent = null;
            }
        }

        public View ViewAt(int index)
        {
            return viewList.Get(index);
        }

        public int IndexOf(View child)
        {
            return viewList.IndexOf(child);
        }

        public int ChildCount()
        {
            return viewList.Count();
        }

        public View Parent()
        {
            return parent;
        }

        #endregion

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
                View first = ViewAt(0);
                float left = first.x;
                float right = left + first.width;
                float top = first.y;
                float bottom = top + first.height;

                for (int i = 1; i < ChildCount(); ++i)
                {
                    View e = ViewAt(i);
                    left = Math.Min(left, e.x);
                    right = Math.Max(right, e.x + e.width);
                    top = Math.Min(top, e.y);
                    bottom = Math.Max(bottom, e.y + e.height);
                }

                for (int i = 0; i < ChildCount(); ++i)
                {
                    View e = ViewAt(i);
                    e.x = horizontal ? leftBorder + e.x - left : e.x;
                    e.y = vertical ? topBorder + e.y - top : e.y;
                }

                width = horizontal ? leftBorder + right - left + rightBorder : width;
                height = vertical ? topBorder + bottom - top + bottomBorder : height;
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
                total += ViewAt(i).width + (i < ChildCount() - 1 ? indent : 0);
            }

            float pos = 0.5f * align * (width - total);
            for (int i = 0; i < ChildCount(); ++i)
            {
                View e = ViewAt(i);
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
                total += ViewAt(i).height + (i < ChildCount() - 1 ? indent : 0);
            }

            float pos = 0.5f * align * (height - total);
            for (int i = 0; i < ChildCount(); ++i)
            {
                View e = ViewAt(i);
                e.y = pos;
                pos += e.height + indent;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Align

        public void SetAlign(float alignX, float alignY)
        {
            this.alignX = alignX;
            this.alignY = alignY;
        }

        public void SetParentAlign(float alignX, float alignY)
        {
            this.parentAlignX = alignX;
            this.parentAlignY = alignY;
        }

        #endregion
    }
}
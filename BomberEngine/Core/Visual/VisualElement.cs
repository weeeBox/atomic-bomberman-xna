using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BomberEngine.Core.Visual
{
    public abstract class VisualElement : BaseElement
    {
        public const float ALIGN_MIN = 0.0f;
        public const float ALIGN_CENTER = 0.5f;
        public const float ALIGN_MAX = 1.0f;

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

        protected VisualElement parent;
        protected VisualElementList childList;

        public VisualElement()
            : this(0, 0)
        {
        }

        public VisualElement(int width, int height)
            : this(0, 0, width, height)
        {
        }

        public VisualElement(float x, float y, int width, int height)
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

            childList = VisualElementList.Null;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Update

        public override void Update(float delta)
        {
            childList.Update(delta);
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
            childList.Draw(context);
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

        public void AddChild(VisualElement child)
        {
            if (childList.IsNull())
            {
                childList = new VisualElementList();
            }

            if (child.parent != null)
            {
                parent.RemoveChild(child);
            }

            childList.Add(child);
            child.parent = this;
        }

        public void RemoveChild(VisualElement child)
        {
            if (childList.Count() > 0)
            {
                childList.Remove(child);
                child.parent = null;
            }
        }

        public VisualElement ChildAt(int index)
        {
            return childList.Get(index);
        }

        public int IndexOf(VisualElement child)
        {
            return childList.IndexOf(child);
        }

        public int ChildCount()
        {
            return childList.Count();
        }

        public VisualElement GetParent()
        {
            return parent;
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
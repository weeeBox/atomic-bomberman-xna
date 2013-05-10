using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BomberEngine.Core.Visual
{
    public abstract class GameObject : BaseObject
    {
        public const float ALIGN_MIN = 0.0f;
        public const float ALIGN_CENTER = 0.5f;
        public const float ALIGN_MAX = 1.0f;

        public float x;
        public float y;
        
        public int width;
        public int height;

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

        private GameObject parent;

        private GameObjectList childList;

        public GameObject()
            : this(0, 0)
        {
        }

        public GameObject(int width, int height)
            : this(0, 0, width, height)
        {
        }

        public GameObject(float x, float y, int width, int height)
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

            childList = GameObjectList.Null;
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

        public virtual void PreDraw(Context context)
        {
            // align to parent
            translateX = x - width * alignX;
            translateY = y - height * alignY;

            if (parent != null)
            {
                translateX += parent.translateX + parent.width * parentAlignX;
                translateY += parent.translateY + parent.height * parentAlignY;
            }

            bool changeScale = (scaleX != 1.0 || scaleY != 1.0);
            bool changeRotation = (rotation != 0.0);
            bool changeTranslate = (translateX != 0.0 || translateY != 0.0);

            // apply transformations
            if (changeTranslate || changeRotation || changeScale)
            {
                context.PushMatrix();

                if (changeRotation || changeScale)
                {
                    float rotationOffsetX = translateX + (width >> 1) + rotationCenterX;
                    float rotationOffsetY = translateY + (height >> 1) + rotationCenterY;

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

        public virtual void PostDraw(Context context)
        {
            DrawChildren(context);
            RestoreTransformations(context);
        }

        public void RestoreTransformations(Context context)
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

        public void AddChild(GameObject child)
        {
            if (childList.IsNull())
            {
                childList = new GameObjectList();
            }
            childList.Add(child);
        }

        public void RemoveChild(GameObject child)
        {
            if (childList.Count() > 0)
            {
                childList.Remove(child);
            }
        }

        public GameObject getParent()
        {
            return parent;
        }

        public void SetParent(GameObject parent)
        {
            this.parent = parent;
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
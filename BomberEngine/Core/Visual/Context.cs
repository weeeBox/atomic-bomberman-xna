using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Assets.Types;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BomberEngine.Debugging;
using BomberEngine.Game;

namespace BomberEngine.Core.Visual
{
    public enum BlendMode
    {
        AlphaBlend,
        NonPremultiplied,
        Additive,
        Opaque,
    }

    public class Context
    {
        enum BatchMode
        {
            None,
            Sprite,
            Geometry,
        }

        private GraphicsDevice graphicsDevice;

        private SpriteBatch spriteBatch;
        private BasicEffect basicEffect;

        private BatchMode batchMode = BatchMode.None;
        private Matrix matrix;
        public Color drawColor;
        private BlendMode blendMode = BlendMode.AlphaBlend;

        private Stack<Matrix> matrixStack = new Stack<Matrix>();

        private Color transpColor = new Color(0, 0, 0, 0);
        private Vector2 zeroVector = new Vector2(0, 0);
        private Camera camera;

        private SpriteFont systemFont;

        private VertexPositionColor[] vertexData = new VertexPositionColor[128];
        private short[] indexData = new short[128];

        private void BeginSpriteBatch(SpriteBatch sb, BlendMode blendMode, Matrix m, BatchMode mode)
        {
            Debug.Assert(mode != BatchMode.None);

            if (mode == BatchMode.Sprite)
            {
                BlendState blendState = toBlendState(blendMode);
                sb.Begin(SpriteSortMode.Immediate, blendState, null, null, null, null, m);
            }
            else if (mode == BatchMode.Geometry)
            {
                basicEffect.World = Matrix.Multiply(camera.WorldMatrix, m);
                basicEffect.CurrentTechnique.Passes[0].Apply();
            }
            batchMode = mode;
        }

        private BlendState toBlendState(BlendMode mode)
        {
            switch (mode)
            {
                case BlendMode.AlphaBlend:
                    return BlendState.AlphaBlend;
                case BlendMode.Additive:
                    return BlendState.Additive;
                case BlendMode.Opaque:
                    return BlendState.Opaque;
                case BlendMode.NonPremultiplied:
                    return BlendState.NonPremultiplied;
                default:
                    throw new NotImplementedException();
            }
        }

        private SpriteBatch GetSpriteBatch(BatchMode mode)
        {
            if (batchMode != mode)
            {
                EndBatch();
                BeginSpriteBatch(spriteBatch, blendMode, matrix, mode);
            }
            return spriteBatch;
        }

        private void EndBatch()
        {
            if (batchMode == BatchMode.Geometry)
            {
                basicEffect.World = Matrix.Identity;
                basicEffect.CurrentTechnique.Passes[0].Apply();
            }
            else if (batchMode == BatchMode.Sprite)
            {
                spriteBatch.End();
            }

            batchMode = BatchMode.None;
        }

        public void SetBlendMode(BlendMode newBlendMode)
        {
            if (blendMode != newBlendMode)
            {
                EndBatch();
                blendMode = newBlendMode;
            }
        }

        public void SetColor(Color color)
        {
            drawColor = color;
        }

        public BlendMode GetBlendMode()
        {
            return blendMode;
        }

        public void SetMatrix(Matrix _matrix)
        {
            matrix = _matrix;
            EndBatch();
        }

        public void SetSystemFont(SpriteFont font)
        {
            systemFont = font;
        }

        public void Begin(GraphicsDevice gd)
        {
            Debug.Assert(batchMode == BatchMode.None, "Bad batch mode: " + batchMode);

            matrixStack.Clear();
            matrix = Matrix.Identity;

            drawColor = Color.White;
            blendMode = BlendMode.AlphaBlend;

            if (graphicsDevice != gd)
            {
                graphicsDevice = gd;
                spriteBatch = new SpriteBatch(graphicsDevice);
                basicEffect = new BasicEffect(graphicsDevice);

                int width = gd.Viewport.Width;
                int height = gd.Viewport.Height;

                Matrix worldMatrix = Matrix.Identity;
                Matrix viewMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
                Matrix projection = Matrix.CreateOrthographicOffCenter(0.0f, width, height, 0, 1.0f, 1000.0f);
                camera = new Camera(worldMatrix, viewMatrix, projection);

                basicEffect.World = worldMatrix;
                basicEffect.View = viewMatrix;
                basicEffect.Projection = projection;
                basicEffect.VertexColorEnabled = true;
            }
        }

        public void End()
        {
            EndBatch();
        }

        public void PushMatrix()
        {
            matrixStack.Push(matrix);
        }

        public void PopMatrix()
        {
            EndBatch();
            matrix = matrixStack.Pop();
        }

        public void SetIdentity()
        {
            EndBatch();
            matrix = Matrix.Identity;
        }

        private void AddTransform(Matrix t)
        {
            EndBatch();
            matrix = Matrix.Multiply(t, matrix);
        }

        public void Translate(float tx, float ty)
        {
            AddTransform(Matrix.CreateTranslation(tx, ty, 0.0f));
        }

        public void Rotate(float rad, float ax, float ay, float az)
        {
            Matrix r;
            if (ax == 1 && ay == 0 && az == 0)
                r = Matrix.CreateRotationX(rad);
            else if (ax == 0 && ay == 1 && az == 0)
                r = Matrix.CreateRotationY(rad);
            else if (ax == 0 && ay == 0 && az == 1)
                r = Matrix.CreateRotationZ(rad);
            else
                throw new NotImplementedException();

            AddTransform(r);
        }

        public void Scale(float sx, float sy, float sz)
        {
            Matrix r = Matrix.CreateScale(sx, sy, sz);
            AddTransform(r);
        }

        public void DrawString(float x, float y, String text)
        {   
            DrawString(systemFont, x, y, text);
        }

        public void DrawString(SpriteFont font, float x, float y, String text)
        {
            GetSpriteBatch(BatchMode.Sprite).DrawString(font, text, new Vector2((float)x, (float)y), drawColor);
        }

        public void DrawString(SpriteFont font, float x, float y, StringBuilder text)
        {
            GetSpriteBatch(BatchMode.Sprite).DrawString(font, text, new Vector2((float)x, (float)y), drawColor);
        }

        public void DrawImage(TextureImage tex, float x, float y)
        {
            GetSpriteBatch(BatchMode.Sprite).Draw(tex.GetTexture(), new Vector2(x, y), drawColor);
        }

        public void DrawImage(TextureImage tex, float x, float y, float opacity)
        {
            GetSpriteBatch(BatchMode.Sprite).Draw(tex.GetTexture(), new Vector2(x, y), new Color(1.0f, 1.0f, 1.0f, opacity));
        }

        public void DrawImage(TextureImage tex, float x, float y, Color color)
        {
            GetSpriteBatch(BatchMode.Sprite).Draw(tex.GetTexture(), new Vector2(x, y), color);
        }

        public void DrawImagePart(TextureImage tex, Rectangle src, float x, float y)
        {
            GetSpriteBatch(BatchMode.Sprite).Draw(tex.GetTexture(), new Vector2(x, y), src, drawColor);
        }

        public void DrawImagePart(TextureImage tex, Rectangle src, float x, float y, Color dc, float size)
        {
            GetSpriteBatch(BatchMode.Sprite).Draw(tex.GetTexture(), new Vector2(x, y), src, drawColor);
        }

        public void DrawImage(TextureImage tex, float x, float y, SpriteEffects flip)
        {
            GetSpriteBatch(BatchMode.Sprite).Draw(tex.GetTexture(), new Vector2(x, y), null, drawColor, 0.0f, Vector2.Zero, 1.0f, flip, 0.0f);
        }

        public void DrawImage(TextureImage tex, ref Vector2 position, ref Color color, float rotation, ref Vector2 origin, ref Vector2 scale, ref Vector2 flip)
        {
            SpriteEffects flipEffects = flip.X == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (flip.Y == 1)
                flipEffects |= SpriteEffects.FlipVertically;

            GetSpriteBatch(BatchMode.Sprite).Draw(tex.GetTexture(), position, null, color, rotation, origin, scale, flipEffects, 0.0f);
        }

        public void DrawImage(TextureImage tex, ref Rectangle src, ref Vector2 position, ref Color color, float rotation, ref Vector2 origin, ref Vector2 scale, ref Vector2 flip)
        {
            SpriteEffects flipEffects = flip.X == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (flip.Y == 1)
                flipEffects |= SpriteEffects.FlipVertically;

            GetSpriteBatch(BatchMode.Sprite).Draw(tex.GetTexture(), position, src, color, rotation, origin, scale, flipEffects, 0.0f);
        }

        public void DrawScaledImage(TextureImage tex, float x, float y, float scaleX, float scaleY)
        {
            Vector2 origin = new Vector2(0.5f * tex.GetWidth(), 0.5f * tex.GetHeight());
            Vector2 scale = new Vector2(scaleX, scaleY);
            GetSpriteBatch(BatchMode.Sprite).Draw(tex.GetTexture(), new Vector2(x, y), null, drawColor, 0.0f, origin, scale, SpriteEffects.None, 0.0f);
        }

        public void DrawScaledImage(TextureImage tex, float x, float y, float scale)
        {
            Vector2 origin = new Vector2(0.5f * tex.GetWidth(), 0.5f * tex.GetHeight());
            GetSpriteBatch(BatchMode.Sprite).Draw(tex.GetTexture(), new Vector2(x, y), null, drawColor, 0.0f, origin, scale, SpriteEffects.None, 0.0f);
        }

        public void DrawImageRotated(TextureImage tex, float x, float y, Vector2 origin, float rotation)
        {
            GetSpriteBatch(BatchMode.Sprite).Draw(tex.GetTexture(), new Vector2(x, y), null, drawColor, rotation, origin, 1.0f, SpriteEffects.None, 0.0f);
        }

        public void DrawImageTiled(TextureImage tex, ref Rectangle src, ref Rectangle dest)
        {
            // TODO: implement with texture repeat
            int destWidth = dest.Width;
            int destHeight = dest.Height;
            int srcWidth = src.Width;
            int srcHeight = src.Height;
            int numTilesX = destWidth / srcWidth + (destWidth % srcWidth != 0 ? 1 : 0);
            int numTilesY = destHeight / srcHeight + (destHeight % srcHeight != 0 ? 1 : 0);
            int x = dest.X;
            int y = dest.Y;
            for (int tileY = 0; tileY < numTilesY; ++tileY)
            {
                for (int tileX = 0; tileX < numTilesX; ++tileX)
                {
                    DrawImagePart(tex, src, x, y);
                    x += srcWidth;
                }
                y += srcHeight;
            }
        }

        public void DrawCircle(float x, float y, float r, Color color)
        {
            GetSpriteBatch(BatchMode.Geometry);

            int numVertex = vertexData.Length;
            float da = MathHelper.TwoPi / (numVertex - 1);
            float angle = 0;
            for (int i = 0; i < numVertex - 1; ++i)
            {
                float vx = (float)(x + r * Math.Cos(angle));
                float vy = (float)(y + r * Math.Sin(angle));
                vertexData[i] = new VertexPositionColor(new Vector3(vx, vy, 0), color);
                angle += da;
            }
            vertexData[numVertex - 1] = vertexData[0];
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertexData, 0, numVertex - 1);
        }

        public void DrawRect(float x, float y, float width, float height, Color color)
        {
            GetSpriteBatch(BatchMode.Geometry);

            vertexData[0] = new VertexPositionColor(new Vector3(x, y, 0), color);
            vertexData[1] = new VertexPositionColor(new Vector3(x + width, y, 0), color);
            vertexData[2] = new VertexPositionColor(new Vector3(x + width, y + height, 0), color);
            vertexData[3] = new VertexPositionColor(new Vector3(x, y + height, 0), color);

            indexData[0] = 0;
            indexData[1] = 1;
            indexData[2] = 2;
            indexData[3] = 3;
            indexData[4] = 0;

            graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineStrip, vertexData, 0, 4, indexData, 0, 4);
        }

        public void DrawLine(float x1, float y1, float x2, float y2, Color color)
        {
            GetSpriteBatch(BatchMode.Geometry);

            vertexData[0] = new VertexPositionColor(new Vector3(x1, y1, 0), color);
            vertexData[1] = new VertexPositionColor(new Vector3(x2, y2, 0), color);

            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertexData, 0, 1);
        }

        public void FillRect(float x, float y, float width, float height, Color color)
        {
            GetSpriteBatch(BatchMode.Geometry);

            vertexData[0] = new VertexPositionColor(new Vector3(x, y, 0), color);
            vertexData[1] = new VertexPositionColor(new Vector3(x + width, y, 0), color);
            vertexData[2] = new VertexPositionColor(new Vector3(x, y + height, 0), color);
            vertexData[3] = new VertexPositionColor(new Vector3(x + width, y + height, 0), color);

            graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertexData, 0, 2);
        }

        public void Clear(Color color)
        {
            EndBatch();
            graphicsDevice.Clear(color);
        }
    }
}

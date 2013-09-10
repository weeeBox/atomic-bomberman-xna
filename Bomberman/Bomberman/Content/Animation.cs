using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Assets;
using BomberEngine.Core.Assets.Types;
using BomberEngine.Debugging;
using BomberEngine.Core.Visual;
using Microsoft.Xna.Framework;

namespace Bomberman.Content
{
    public class Animation : Asset
    {
        public String name;
        public TextureImage texture;

        public AnimationFrame[] frames;

        public Animation(String name)
        {
            this.name = name;
        }

        public void Draw(Context context, int frameIndex, float x, float y, Color color)
        {
            Rectangle src;
            int ox = frames[frameIndex].ox;
            int oy = frames[frameIndex].oy;

            src.X = frames[frameIndex].x;
            src.Y = frames[frameIndex].y;
            src.Width = frames[frameIndex].w;
            src.Height = frames[frameIndex].h;

            context.DrawImagePart(texture, src, x - ox, y - oy, color);
        }

        protected override void OnDispose()
        {
            if (!texture.IsDisposed())
            {
                texture.Dispose();
            }
        }
    }

    public struct AnimationFrame
    {
        public int x;
        public int y;
        public int ox;
        public int oy;
        public int w;
        public int h;
        public float duration;
    }
}

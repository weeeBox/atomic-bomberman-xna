using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Assets;
using BomberEngine.Core.Assets.Types;
using BomberEngine.Debugging;

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

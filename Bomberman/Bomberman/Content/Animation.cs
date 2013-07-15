using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Assets;
using BomberEngine.Core.Assets.Types;

namespace Bomberman.Content
{
    public class Animation : Asset
    {
        public String name;
        public AnimationGroup[] groups;
        public TextureImage texture;

        public Animation(String name)
        {
            this.name = name;
        }

        protected override void OnDispose()
        {
            texture.Dispose();
        }
    }

    public class AnimationGroup
    {
        public String name;
        public AnimationFrame[] frames;

        public AnimationGroup(String name)
        {
            this.name = name;
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
    }
}

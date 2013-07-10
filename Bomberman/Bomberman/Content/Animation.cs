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

        protected override void OnDispose()
        {
            texture.Dispose();
        }
    }

    public class AnimationGroup
    {
        public String name;
        public AnimationFrame[] frames;
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

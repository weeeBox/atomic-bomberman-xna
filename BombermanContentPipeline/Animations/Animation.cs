using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BombermanContentPipeline.Animations
{
    public class Animation
    {
        public String name;
        public AnimationGroup[] groups;
        public byte[] textureBytes;
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

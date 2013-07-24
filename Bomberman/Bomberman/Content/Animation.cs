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
        public IDictionary<String, AnimationGroup> groups;
        public TextureImage texture;

        public Animation(String name)
        {
            this.name = name;
            groups = new Dictionary<String, AnimationGroup>();
        }

        public void Add(AnimationGroup group)
        {
            String name = group.name;
            Debug.Assert(name != null);
            Debug.Assert(!groups.ContainsKey(name));

            groups[name] = group;
        }

        public AnimationGroup Group(String name)
        {
            AnimationGroup group;
            if (groups.TryGetValue(name, out group))
            {
                return group;
            }

            return null;
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
        public float duration;
    }
}

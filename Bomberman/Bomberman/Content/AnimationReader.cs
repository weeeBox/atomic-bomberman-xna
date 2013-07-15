using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BomberEngine.Game;
using BomberEngine.Core.Assets.Types;
using BomberEngine.Core.Assets;
using System.IO;

namespace Bomberman.Content
{
    public class AnimationReader : AssetBinaryReader
    {
        protected override Asset Read(BinaryReader input)
        {
            String name = input.ReadString();
            Animation animation = new Animation(name);

            int groupsCount = input.ReadInt32();
            AnimationGroup[] groups = new AnimationGroup[groupsCount];

            for (int groupIndex = 0; groupIndex < groupsCount; ++groupIndex)
            {
                String groupName = input.ReadString();
                AnimationGroup g = new AnimationGroup(groupName);
                
                int framesCount = input.ReadInt32();

                AnimationFrame[] frames = new AnimationFrame[framesCount];
                for (int frameIndex = 0; frameIndex < frames.Length; ++frameIndex)
                {
                    frames[frameIndex].x = input.ReadInt32();
                    frames[frameIndex].y = input.ReadInt32();
                    frames[frameIndex].ox = input.ReadInt32();
                    frames[frameIndex].oy = input.ReadInt32();
                    frames[frameIndex].w = input.ReadInt32();
                    frames[frameIndex].h = input.ReadInt32();
                }

                g.frames = frames;
            }
            animation.groups = groups;

            return animation;
        }
    }
}

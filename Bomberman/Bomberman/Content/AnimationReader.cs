using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BomberEngine.Game;
using BomberEngine.Core.Assets.Types;

namespace Bomberman.Content
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content
    /// Pipeline to read the specified data type from binary .xnb format.
    /// 
    /// Unlike the other Content Pipeline support classes, this should
    /// be a part of your main game project, and not the Content Pipeline
    /// Extension Library project.
    /// </summary>
    public class AnimationReader : ContentTypeReader<Animation>
    {
        protected override Animation Read(ContentReader input, Animation existingInstance)
        {
            Animation animation;
            if (existingInstance != null)
            {
                animation = existingInstance;
            }
            else
            {
                animation = new Animation();
            }

            String textureName = input.ReadString();
            animation.texture = Application.Assets().LoadAsset<TextureImage>(textureName);

            animation.name = input.ReadString();
            int groupsCount = input.ReadInt32();

            AnimationGroup[] groups = new AnimationGroup[groupsCount];
            for (int groupIndex = 0; groupIndex < groupsCount; ++groupIndex)
            {
                AnimationGroup g = new AnimationGroup();
                g.name = input.ReadString();
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

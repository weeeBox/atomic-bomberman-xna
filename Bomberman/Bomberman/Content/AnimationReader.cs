using System;
using System.IO;
using BomberEngine;
using Microsoft.Xna.Framework.Graphics;

namespace Bomberman.Content
{
    public class AnimationReader : AssetBinaryReader
    {
        protected override Asset Read(BinaryReader input)
        {
            String name = input.ReadString();
            Animation animation = new Animation(name);
                
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
                frames[frameIndex].duration = input.ReadInt32() * 0.001f;
            }

            animation.frames = frames;

            int textureSize = input.ReadInt32();
            byte[] data = input.ReadBytes(textureSize);

            using (MemoryStream stream = new MemoryStream(data))
            {
                Texture2D texture = Texture2D.FromStream(Runtime.graphicsDevice, stream);
                animation.texture = new TextureImage(texture);
            }

            return animation;
        }
    }
}

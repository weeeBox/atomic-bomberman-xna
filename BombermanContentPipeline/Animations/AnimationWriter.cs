using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace BombermanContentPipeline.Animations
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class AnimationWriter : ContentTypeWriter<Animation>
    {
        protected override void Write(ContentWriter output, Animation animation)
        {
            output.Write(animation.textureName);
            output.Write(animation.name);

            AnimationGroup[] groups = animation.groups;
            output.Write(groups.Length);
            foreach (AnimationGroup g in groups)
            {
                output.Write(g.name);

                AnimationFrame[] frames = g.frames;
                output.Write(frames.Length);
                for (int i = 0; i < frames.Length; ++i)
                {
                    Write(output, ref frames[i]);
                }
            }
        }

        private void Write(ContentWriter output, ref AnimationFrame frame)
        {
            output.Write(frame.x);
            output.Write(frame.y);
            output.Write(frame.ox);
            output.Write(frame.oy);
            output.Write(frame.w);
            output.Write(frame.h);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Bomberman.Content.AnimationReader, Bomberman";
        }
    }
}

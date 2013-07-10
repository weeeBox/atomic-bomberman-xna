using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Xml.Linq;
using System.IO;

namespace BombermanContentPipeline.Animations
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// 
    /// TODO: change the ContentImporter attribute to specify the correct file
    /// extension, display name, and default processor for this importer.
    /// </summary>
    [ContentImporter(".anim", DisplayName = "Animation Importer")]
    public class AnimationImporter : ContentImporter<Animation>
    {
        public override Animation Import(string path, ContentImporterContext context)
        {
            // animations data
            XElement root = XElement.Load(path);

            Animation animation = new Animation();
            animation.name = (String)root.Attribute("name");

            // groups
            IEnumerable<XElement> animationElements = root.Elements("animation");
            AnimationGroup[] groups = new AnimationGroup[CountElements(animationElements)];
            int groupIndex = 0;
            foreach (XElement animationElement in animationElements)
            {
                AnimationGroup group = new AnimationGroup();
                group.name = (String)animationElement.Attribute("name");

                // frames
                IEnumerable<XElement> frameElements = animationElement.Elements("frame");
                AnimationFrame[] frames = new AnimationFrame[CountElements(frameElements)];
                int frameIndex = 0;
                foreach (XElement frameElement in frameElements)
                {
                    frames[frameIndex].x = (int)frameElement.Attribute("x");
                    frames[frameIndex].y = (int)frameElement.Attribute("y");
                    frames[frameIndex].ox = (int)frameElement.Attribute("ox");
                    frames[frameIndex].oy = (int)frameElement.Attribute("oy");
                    frames[frameIndex].w = (int)frameElement.Attribute("w");
                    frames[frameIndex].h = (int)frameElement.Attribute("h");
                    ++frameIndex;
                }

                group.frames = frames;
                ++groupIndex;
            }
            animation.groups = groups;

            // texture
            String parentPath = Directory.GetParent(path).FullName;
            String textureFilename = (String)root.Attribute("file");

            animation.textureBytes = File.ReadAllBytes(Path.Combine(parentPath, textureFilename));

            return animation;
        }

        private int CountElements<T>(IEnumerable<T> enumerable)
        {
            int count = 0;
            foreach (T e in enumerable)
            {
                ++count;
            }

            return count;
        }
    }
}

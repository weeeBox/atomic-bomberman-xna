using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using BombermanCommon.Resources.Values;

namespace BombermanContentPipeline.Values
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class ValuesWriter : ContentTypeWriter<ValuesList>
    {
        protected override void Write(ContentWriter output, ValuesList valuesList)
        {
            List<ValuePair> list = valuesList.list;
            output.Write(list.Count);
            foreach (ValuePair p in list)
            {
                output.Write(p.name);
                output.Write(p.value);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Bomberman.Content.ValuesReader, Bomberman";
        }
    }
}

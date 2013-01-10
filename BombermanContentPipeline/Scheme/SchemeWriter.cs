using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using BombermanCommon.Resources.Scheme;

namespace BombermanContentPipeline.Scheme
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class SchemeWriter : ContentTypeWriter<SchemeResource>
    {
        protected override void Write(ContentWriter output, SchemeResource scheme)
        {
            output.Write(scheme.Version);
            output.Write(scheme.Name);

            FieldData fieldData = scheme.FieldData;
            output.Write(fieldData.Width);
            output.Write(fieldData.Height);

            EnumBlocks[] blocks = fieldData.Data;
            for (int i = 0; i < blocks.Length; ++i)
            {
                output.Write((int)blocks[i]);
            }

            PlayerInfo[] playerInfo = scheme.PlayerLocations;
            output.Write(playerInfo.Length);
            for (int i = 0; i < playerInfo.Length; ++i)
            {
                output.Write(playerInfo[i].x);
                output.Write(playerInfo[i].y);
                output.Write(playerInfo[i].team);
            }

            PowerupInfo[] powerupInfo = scheme.PowerupInfo;
            output.Write(powerupInfo.Length);
            for (int i = 0; i < powerupInfo.Length; ++i)
            {
                output.Write(powerupInfo[i].bornWith);
                output.Write(powerupInfo[i].hasOverride);
                output.Write(powerupInfo[i].overrideValue);
                output.Write(powerupInfo[i].forbidden);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Bomberman.Content.SchemeReader, Bomberman";
        }
    }
}

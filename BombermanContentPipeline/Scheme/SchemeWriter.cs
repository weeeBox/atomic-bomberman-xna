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
    [ContentTypeWriter]
    public class SchemeWriter : ContentTypeWriter<SchemeInfo>
    {
        protected override void Write(ContentWriter output, SchemeInfo scheme)
        {   
            output.Write(scheme.name);
            output.Write(scheme.brickDensity);

            FieldData fieldData = scheme.fieldData;
            output.Write(fieldData.GetWidth());
            output.Write(fieldData.GetHeight());

            FieldBlocks[] blocks = fieldData.GetDataArray();
            for (int i = 0; i < blocks.Length; ++i)
            {
                output.Write((int)blocks[i]);
            }

            PlayerLocationInfo[] playerInfo = scheme.playerLocations;
            output.Write(playerInfo.Length);
            for (int i = 0; i < playerInfo.Length; ++i)
            {
                output.Write(playerInfo[i].x);
                output.Write(playerInfo[i].y);
                output.Write(playerInfo[i].team);
            }

            PowerupInfo[] powerupInfo = scheme.powerupInfo;
            output.Write(powerupInfo.Length);
            for (int i = 0; i < powerupInfo.Length; ++i)
            {
                output.Write(powerupInfo[i].powerupIndex);
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

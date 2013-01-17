using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BombermanCommon.Resources.Scheme;

// TODO: replace this with the type you want to read.

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
    public class SchemeReader : ContentTypeReader<SchemeAsset>
    {
        protected override SchemeAsset Read(ContentReader input, SchemeAsset existingInstance)
        {
            SchemeAsset scheme = new SchemeAsset();
            scheme.name = input.ReadString();

            // field
            int fieldWidth = input.ReadInt32();
            int fieldHeight = input.ReadInt32();

            FieldData fieldData = new FieldData(fieldWidth, fieldHeight);
            for (int y = 0; y < fieldHeight; ++y)
            {
                for (int x = 0; x < fieldWidth; ++x)
                {
                    int block = input.ReadInt32();
                    fieldData.Set(x, y, (FieldBlocks)block);
                }
            }
            scheme.fieldData = fieldData;

            // players locations
            int playerInfoCount = input.ReadInt32();
            PlayerLocationInfo[] playerLocations = new PlayerLocationInfo[playerInfoCount];
            for (int i = 0; i < playerInfoCount; ++i)
            {
                PlayerLocationInfo playerInfo = new PlayerLocationInfo();
                playerInfo.x = input.ReadInt32();
                playerInfo.y = input.ReadInt32();
                playerInfo.team = input.ReadInt32();

                playerLocations[i] = playerInfo;
            }
            scheme.playerLocations = playerLocations;

            // powerups
            int powerupInfoCount = input.ReadInt32();
            PowerupInfo[] powerupInfo = new PowerupInfo[powerupInfoCount];
            for (int i = 0; i < powerupInfoCount; ++i)
            {
                PowerupInfo info = new PowerupInfo();
                info.bornWith = input.ReadBoolean();
                info.hasOverride = input.ReadBoolean();
                info.overrideValue = input.ReadInt32();
                info.forbidden = input.ReadBoolean();

                powerupInfo[i] = info;
            }
            scheme.powerupInfo = powerupInfo;

            return scheme;
        }
    }
}

using System.IO;
using BomberEngine;
using BombermanCommon.Resources.Scheme;

namespace Bomberman.Content
{
    public class SchemeReader : AssetBinaryReader
    {
        protected override Asset Read(BinaryReader reader)
        {
            Scheme scheme = new Scheme();

            scheme.name = reader.ReadString();
            scheme.brickDensity = reader.ReadInt32();

            int width = reader.ReadInt32();
            int height = reader.ReadInt32();

            scheme.fieldData = new FieldData(width, height);
            
            FieldBlocks[] blocks = scheme.fieldData.GetDataArray();
            for (int i = 0; i < blocks.Length; ++i)
            {
                blocks[i] = (FieldBlocks)reader.ReadInt32();
            }

            int playersCount = reader.ReadInt32();
            PlayerLocationInfo[] playerLocations = new PlayerLocationInfo[playersCount];
            
            for (int i = 0; i < playerLocations.Length; ++i)
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                int team = reader.ReadInt32();

                playerLocations[i] = new PlayerLocationInfo(i, x, y, team);
            }
            scheme.playerLocations = playerLocations;

            int powerupsCount = reader.ReadInt32();
            PowerupInfo[] powerupInfo = new PowerupInfo[powerupsCount];
            for (int i = 0; i < powerupInfo.Length; ++i)
            {
                powerupInfo[i].powerupIndex = reader.ReadInt32();
                powerupInfo[i].bornWith = reader.ReadBoolean();
                powerupInfo[i].hasOverride = reader.ReadBoolean();
                powerupInfo[i].overrideValue = reader.ReadInt32();
                powerupInfo[i].forbidden = reader.ReadBoolean();
            }

            scheme.powerupInfo = powerupInfo;

            return scheme;
        }
    }
}

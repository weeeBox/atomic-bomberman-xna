using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Assets;
using BombermanCommon.Resources.Scheme;

namespace Bomberman.Content
{
    public class Scheme : Asset
    {
        public String name;
        public FieldData fieldData;
        public PlayerLocationInfo[] playerLocations;
        public PowerupInfo[] powerupInfo;
        public int brickDensity;

        public Scheme()
        {
            
        }

        public String GetName()
        {
            return name;
        }

        public FieldData GetFieldData()
        {
            return fieldData;
        }

        public PlayerLocationInfo[] GetPlayerLocations()
        {
            return playerLocations;
        }

        public PowerupInfo[] GetPowerupInfo()
        {
            return powerupInfo;
        }

        public int GetBrickDensity()
        {
            return brickDensity;
        }

        public int GetMaxPlayersCount()
        {
            return playerLocations.Length;
        }
    }
}

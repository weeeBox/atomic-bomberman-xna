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
        private SchemeInfo info;

        public Scheme(SchemeInfo info)
        {
            this.info = info;
        }

        public String GetName()
        {
            return info.name;
        }

        public FieldData GetFieldData()
        {
            return info.fieldData;
        }

        public PlayerLocationInfo[] GetPlayerLocations()
        {
            return info.playerLocations;
        }

        public PowerupInfo[] GetPowerupInfo()
        {
            return info.powerupInfo;
        }

        public int GetBrickDensity()
        {
            return info.brickDensity;
        }

        protected override void OnDispose()
        {
        }
    }
}

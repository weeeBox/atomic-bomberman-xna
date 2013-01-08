using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BombermanLiveCommon.Resources.Scheme
{
    public class SchemeInfo
    {
        private int version;
        private string name;
        private FieldData fieldData;
        private PlayerInfo[] playerLocations;
        private PowerupInfo[] powerupInfo;

        public int Version
        {
            get { return version; }
            set { version = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public FieldData FieldData
        {
            get { return fieldData; }
            set { fieldData = value; }
        }

        public PlayerInfo[] PlayerLocations
        {
            get { return playerLocations; }
            set { playerLocations = value; }
        }

        public PowerupInfo[] PowerupInfo
        {
            get { return powerupInfo; }
            set { powerupInfo = value; }
        }
    }
}


namespace BombermanCommon.Resources.Scheme
{
    public struct PowerupInfo
    {
        public int powerupIndex;
        public bool bornWith;
        public bool hasOverride;
        public int overrideValue;
        public bool forbidden;

        public PowerupInfo(int powerupIndex, bool bornWith, bool hasOverride, int overrideValue, bool forbidden)
        {
            this.powerupIndex = powerupIndex;
            this.bornWith = bornWith;
            this.hasOverride = hasOverride;
            this.overrideValue = overrideValue;
            this.forbidden = forbidden;
        }
    }
}

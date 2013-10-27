
namespace BombermanCommon.Resources.Scheme
{
    public struct PlayerLocationInfo
    {
        public int index;
        public int x;
        public int y;
        public int team;

        public PlayerLocationInfo(int index, int x, int y, int team)
        {
            this.index = index;
            this.x = x;
            this.y = y;
            this.team = team;
        }
    }
}

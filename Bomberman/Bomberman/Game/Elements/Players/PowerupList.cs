using BomberEngine;

namespace Bomberman.Gameplay.Elements.Players
{
    public class PowerupList
    {
        public int[] powerups;
        public int[] maxCount;

        private Player m_player;

        public PowerupList(Player player, int totalCount)
        {
            Assert.IsTrue(player != null);
            m_player = player;

            powerups = new int[totalCount];
            maxCount = new int[totalCount];
        }

        public void Init(int powerupIndex, int count, int max)
        {
            powerups[powerupIndex] = count;
            maxCount[powerupIndex] = max;
        }

        public bool HasPowerup(int powerupIndex)
        {
            return GetCount(powerupIndex) > 0;
        }

        public int GetCount(int powerupIndex)
        {
            return powerups[powerupIndex];
        }

        public void SetCount(int powerupIndex, int count)
        {
            Debug.CheckArgumentRange("powerupIndex", powerupIndex, 0, powerups.Length);
            Debug.CheckArgumentRange("count", count, 0, maxCount[powerupIndex] + 1);
            
            powerups[powerupIndex] = count;
        }

        public bool Inc(int powerupIndex)
        {
            int count = powerups[powerupIndex];
            if (count < maxCount[powerupIndex])
            {
                powerups[powerupIndex] = count + 1;
                return true;
            }

            if (count == 0)
            {
                return true;
            }

            return false;
        }

        public Player player
        {
            get { return m_player; }
        }
    }
}

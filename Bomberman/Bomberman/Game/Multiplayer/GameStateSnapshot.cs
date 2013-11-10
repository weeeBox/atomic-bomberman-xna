using System.Collections.Generic;
using Bomberman.Gameplay.Elements;
using Bomberman.Gameplay.Elements.Cells;
using Bomberman.Gameplay.Elements.Players;

namespace Bomberman.Gameplay.Multiplayer
{
    public class GameStateSnapshot
    {   
        public PlayerState[] players;

        public void SetFrom(List<Player> list)
        {
            players = EnsureArray(players, list.Count);
            for (int i = 0; i < players.Length; ++i)
            {
                players[i].SetFrom(list[i]);
            }
        }

        private PlayerState[] EnsureArray(PlayerState[] array, int length)
        {
            if (array == null || array.Length != length)
            {
                array = new PlayerState[length];
            }
            return array;
        }
    }

    public struct PlayerState
    {
        public Direction direction;
        public float px;
        public float py;
        public float speed;
        public int powerups;
        public int diseases;

        public BombState[] bombs;

        public int flameCount;
        public FlameState[] flames;

        public void SetFrom(Player p)
        {
            direction = p.direction;
            px = p.px;
            py = p.py;
            speed = p.GetSpeed();
            // powerups = p.powerups; // TODO
            // diseases = p.diseases; // TODO

            bombs = EnsureArray(bombs, p.bombs.array.Length);
            for (int i = 0; i < bombs.Length; ++i)
            {
                bombs[i].SetFrom(p.bombs.array[i]);
            }
        }

        private BombState[] EnsureArray(BombState[] array, int length)
        {
            if (array == null || array.Length != length)
            {
                array = new BombState[length];
            }
            return array;
        }
    }

    public struct BombState
    {
        private const int FLAG_JELLY = 1 << 0;
        private const int FLAG_TRIGGER = 1 << 1;

        public bool active;
        public Direction direction;
        public float px;
        public float py;
        public float speed;
        public float timeStamp;
        public int flags;

        public void SetFrom(Bomb b)
        {
            active = b.isActive;
            if (active)
            {
                direction = b.direction;
                px = b.px;
                py = b.py;
                speed = b.GetSpeed();
                flags = 0;
                flags |= b.IsJelly() ? FLAG_JELLY : 0;
                flags |= b.IsTrigger() ? FLAG_TRIGGER : 0;
            }
        }
    }

    public struct FlameState
    {
        public int cx;
        public int cy;
        public float timeStamp;

        public void SetFrom(FlameCell f)
        {
            cx = f.cx;
            cy = f.cy;
        }
    }
}

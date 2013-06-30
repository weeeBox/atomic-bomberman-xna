using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Cells;

namespace Bomberman.Game.Multiplayer
{
    public class GamePacketServer : GamePacket
    {
        GameState gameState;
    }

    public class GameState
    {
        public int playersCount;
        public PlayerState[] players;
    }

    public struct PlayerState
    {
        public Direction direction;
        public float px;
        public float py;
        public float speed;
        public int powerups;
        public int diseases;

        public int bombCount;
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
        }
    }

    public struct BombState
    {
        private const int FLAG_JELLY   = 1 << 0;
        private const int FLAG_TRIGGER = 1 << 1;

        public Direction direction;
        public float px;
        public float py;
        public float speed;
        public float timeStamp;
        public int flags;

        public void SetFrom(Bomb b)
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
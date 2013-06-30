using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements;

namespace Bomberman.Game.Multiplayer
{
    public class ServerGamePacket : GamePacket
    {
        public int playersCount;
        public PlayerState[] players;

        public int bombCount;
        public BombState[] bombs;

        public int flameCount;
        public FlameState[] flames;
    }

    public struct PlayerState
    {
        public Direction direction;
        public float px;
        public float py;
        public int velocity;
        public int powerups;
        public int diseases;
    }

    public struct BombState
    {
        public Direction direction;
        public float px;
        public float py;
        public int velocity;
        public float timeStamp;
        public bool jelly;
        public bool trigger;
    }

    public struct FlameState
    {
        public int cx;
        public int cy;
        public float timeStamp;
    }
}
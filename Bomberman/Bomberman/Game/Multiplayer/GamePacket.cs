
namespace Bomberman.Game.Multiplayer
{
    public abstract class GamePacket
    {
        public int id;
        public int lastAcknowledgedId;
        public float timeStamp;
    }
}

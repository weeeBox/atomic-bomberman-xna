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
        GameStateSnapshot gameState;
    }
}
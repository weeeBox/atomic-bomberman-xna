using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game;

namespace BombermanTests.Mocks
{
    public class GameMock : Game
    {
        public GameMock(int width, int height)
            : base(width, height)
        {
        }
    }
}

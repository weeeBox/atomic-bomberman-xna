using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Gameplay.Elements.Fields;
using BomberEngine;
using Bomberman.Gameplay;

namespace BombermanTests.Mocks
{
    public class FieldMock : Field
    {
        public FieldMock(Game game)
            : base(game, 15, 11)
        {
            MathHelp.InitRandom(0);
        }
    }
}

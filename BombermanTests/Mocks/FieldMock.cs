using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Gameplay.Elements.Fields;
using BomberEngine;

namespace BombermanTests.Mocks
{
    public class FieldMock : Field
    {
        public FieldMock()
            : base(15, 11)
        {
            MathHelp.InitRandom(0);
        }
    }
}

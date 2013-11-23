using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine;
using Bomberman;

namespace BombermanTests.Mocks
{
    public class BmRootControllerMock : BmRootController
    {
        public BmRootControllerMock()
        {
            networkManager = new NetworkManagerMock();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine;

namespace BombermanTests.Mocks
{
    public class ApplicationMock : Application
    {
        public ApplicationMock()
            : base(new ApplicationInfo(640, 480))
        {
            assetManager = CreateAssetManager();
            rootController = CreateRootController();
        }

        protected override AssetManager CreateAssetManager()
        {
            return new AssetManagerMock();
        }

        protected override RootController CreateRootController()
        {
            return new BmRootControllerMock();
        }
    }
}

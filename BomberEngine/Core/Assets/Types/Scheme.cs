using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BombermanCommon.Resources.Scheme;

namespace BomberEngine.Core.Assets.Types
{
    public class Scheme : Asset
    {
        private SchemeInfo info;

        public Scheme(SchemeInfo info)
        {
            this.info = info;
        }

        protected override void OnDispose()
        {   
        }
    }
}

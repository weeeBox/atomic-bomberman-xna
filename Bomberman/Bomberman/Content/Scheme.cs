using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Assets;
using BombermanCommon.Resources.Scheme;

namespace Bomberman.Content
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

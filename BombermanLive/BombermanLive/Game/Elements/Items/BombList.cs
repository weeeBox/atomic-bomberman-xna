using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;

namespace BombermanLive.Game.Elements.Items
{
    public class BombList : Updatable
    {
        private List<Bomb> bombs;

        public BombList()
        {
            bombs = new List<Bomb>();
        }

        public void Update(float delta)
        {
            
        }
    }
}

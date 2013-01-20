using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Bomberman.Game.Elements.Cells;

namespace Bomberman.Game.Elements.Items
{
    public class BombList : Updatable
    {
        private List<Bomb> bombs;

        private int capacity;
        private int maxCapacity;

        public BombList(int capacity, int maxCapacity)
        {   
            bombs = new List<Bomb>(maxCapacity);
            this.capacity = capacity;
            this.maxCapacity = maxCapacity;
        }

        public void Update(float delta)
        {
            
        }
    }
}

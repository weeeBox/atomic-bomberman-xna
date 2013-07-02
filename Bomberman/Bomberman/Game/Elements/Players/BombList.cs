using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Players;
using BomberEngine;
using BomberEngine.Debugging;

namespace Bomberman.Game.Elements.Items
{
    public class BombList
    {
        public Bomb[] array;

        private int maxActiveCount;

        public BombList(Player player, int capacity)
        {
            array = new Bomb[capacity];
            for (int i = 0; i < capacity; ++i)
            {
                array[i] = new Bomb(player);
            }
        }

        public void SetMaxActiveCount(int count)
        {
            Debug.CheckArgumentRange("count", count, 0, array.Length + 1);
            this.maxActiveCount = count;
        }

        public bool IncMaxActiveCount()
        {
            if (maxActiveCount < array.Length)
            {
                ++maxActiveCount;
                return true;
            }

            return false;
        }

        public int GetMaxActiveCount()
        {
            return maxActiveCount;
        }

        public Bomb GetFirstKickedBomb()
        {
            Bomb kickedBomb = null;

            for (int i = 0; i < array.Length; ++i)
            {
                Bomb bomb = array[i];
                if (bomb.active)
                {
                    if (bomb.IsMoving() && (kickedBomb == null || kickedBomb.remains > bomb.remains))
                    {
                        kickedBomb = bomb;
                    }
                }
            }

            return kickedBomb;
        }

        public Bomb GetFirstTriggerBomb()
        {
            Bomb triggerBomb = null;

            for (int i = 0; i < array.Length; ++i)
            {
                Bomb bomb = array[i];
                if (bomb.active)
                {
                    if (bomb.CanTrigger() && (triggerBomb == null || triggerBomb.triggerIndex > bomb.triggerIndex))
                    {
                        triggerBomb = bomb;
                    }
                }
            }

            return triggerBomb;
        }

        public Bomb GetNextBomb()
        {
            Bomb nextBomb = null;
            
            int activeCount = 0;
            for (int i = 0; i < array.Length; ++i)
            {
                Bomb bomb = array[i];
                if (bomb.active)
                {   
                    if (++activeCount == maxActiveCount)
                    {
                        return null;
                    }
                }
                else
                {
                    nextBomb = bomb;
                }
            }

            if (nextBomb != null)
            {
                nextBomb.Activate();
                return nextBomb;
            }

            return null;
        }
    }
}

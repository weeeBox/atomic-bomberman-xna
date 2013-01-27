using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Game.Elements
{
    public class Diseases
    {
        public int m_index;
        public String m_name;

        private Diseases(String name, int index)
        {
            this.m_index = index;
            this.m_name = name;
        }

        public int index
        {
            get { return m_index; }
        }

        public String name
        {
            get { return m_name; }
        }

        public static readonly Diseases MOLASSES = new Diseases("Molasses", 0);         // speed down
        public static readonly Diseases CRACK = new Diseases("Crack", 1);               // speed up
        public static readonly Diseases CONSTIPATION = new Diseases("Constipation", 2); // player can't drop bombs
        public static readonly Diseases POOPS = new Diseases("Poops", 3);               // player drops all bombs he has got
        public static readonly Diseases SHORTFLAME = new Diseases("Short Flame", 4);    // flame length = 1
        public static readonly Diseases CRACKPOOPS = new Diseases("Crack Poops", 5);    // CRACK + POOPS
        public static readonly Diseases SHORTFUZE = new Diseases("Short Fuze", 6);      // shorter fuze time
        public static readonly Diseases SWAP = new Diseases("Swap", 7);                 // two players swap
        public static readonly Diseases REVERSED = new Diseases("Reversed", 8);         // reversed controls
        public static readonly Diseases LEPROSY = new Diseases("Leprosy", 9);           // Lepra ???
        public static readonly Diseases INVISIBLE = new Diseases("Invisible", 10);      // player is invisible
        public static readonly Diseases DUDS = new Diseases("Duds", 11);                // player drops only duds
        public static readonly Diseases HYPERSWAP = new Diseases("Hyperswap", 12);      // all players swap

        public static readonly int Count = 13;

        public static readonly Diseases[] array = 
        {
            MOLASSES,
            CRACK,
            CONSTIPATION,
            POOPS,
            SHORTFLAME,
            CRACKPOOPS,
            SHORTFUZE,
            SWAP,
            REVERSED,
            LEPROSY,
            INVISIBLE,
            DUDS,
            HYPERSWAP,
        };

        public static Diseases FromIndex(int index)
        {
            if (index >= 0 && index < array.Length)
            {
                return array[index];
            }
            return null;
        }
    }
}

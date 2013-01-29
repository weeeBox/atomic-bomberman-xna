using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Game.Elements
{
    public class Diseases
    {
        private int m_index;
        private String m_name;
        private float m_duration;

        private Diseases(String name, int index, float duration)
        {
            this.m_index = index;
            this.m_name = name;
            this.m_duration = duration;
        }

        public int index
        {
            get { return m_index; }
        }

        public String name
        {
            get { return m_name; }
        }

        public float duration
        {
            get { return m_duration; }
        }

        public static readonly Diseases MOLASSES = new Diseases("Molasses", 0, 15.0f);         // speed down
        public static readonly Diseases CRACK = new Diseases("Crack", 1, 15.0f);               // speed up
        public static readonly Diseases CONSTIPATION = new Diseases("Constipation", 2, 15.0f); // player can't drop bombs
        public static readonly Diseases POOPS = new Diseases("Poops", 3, 15.0f);               // player drops all bombs he has got
        public static readonly Diseases SHORTFLAME = new Diseases("Short Flame", 4, 15.0f);    // flame length = 1
        public static readonly Diseases CRACKPOOPS = new Diseases("Crack Poops", 5, 15.0f);    // CRACK + POOPS
        public static readonly Diseases SHORTFUZE = new Diseases("Short Fuze", 6, 15.0f);      // shorter fuze time
        public static readonly Diseases SWAP = new Diseases("Swap", 7, 0.0f);                  // two players swap
        public static readonly Diseases REVERSED = new Diseases("Reversed", 8, 15.0f);         // reversed controls
        public static readonly Diseases LEPROSY = new Diseases("Leprosy", 9, 15.0f);           // Lepra ???
        public static readonly Diseases INVISIBLE = new Diseases("Invisible", 10, 15.0f);      // player is invisible
        public static readonly Diseases DUDS = new Diseases("Duds", 11, 15.0f);                // player drops only duds
        public static readonly Diseases HYPERSWAP = new Diseases("Hyperswap", 12, 0.0f);       // all players swap

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Players;

namespace Bomberman.Game.Elements
{
    public enum Diseases
    {
        MOLASSES = 0, // speed down
        CRACK,        // speed up
        CONSTIPATION, // player can't drop bombs
        POOPS,        // player drops all bombs he has got
        SHORTFLAME,   // flame length = 1
        CRACKPOOPS,   // CRACK + POOPS
        SHORTFUZE,    // shorter fuze time
        SWAP,         // two players swap
        REVERSED,     // reversed controls
        LEPROSY,      // Lepra ???
        INVISIBLE,    // player is invisible
        DUDS,         // player drops only duds
        HYPERSWAP,    // all players swap

        Count
    }

    public class Disease
    {
        private Diseases m_type;
        private String m_name;
        private float m_duration;

        public Disease(String name, Diseases type, float duration)
        {
            this.m_type = type;
            this.m_name = name;
            this.m_duration = duration;
        }

        public int index
        {
            get { return (int)m_type; }
        }

        public Diseases type
        {
            get { return m_type; }
        }

        public String name
        {
            get { return m_name; }
        }

        public float duration
        {
            get { return m_duration; }
        }
    }
}

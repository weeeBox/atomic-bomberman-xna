using System;

namespace Bomberman.Game.Elements
{
    public class Powerups
    {
        public const int None = 0xff;

        public const int Bomb = 0;
        public const int Flame = 1;
        public const int Disease = 2;
        public const int Kick = 3;
        public const int Speed = 4;
        public const int Punch = 5;
        public const int Grab = 6;
        public const int Spooger = 7;
        public const int GoldFlame = 8;
        public const int Trigger = 9;
        public const int Jelly = 10;
        public const int Ebola = 11;
        public const int Random = 12;

        ///////////////////////////////
        public const int Count = 13;

        private static String[] names;

        public static String Name(int powerupIndex)
        {
            if (powerupIndex >= 0 && powerupIndex < Count)
            {
                String[] names = Names();
                return names[powerupIndex];
            }

            return null;
        }

        public static String[] Names()
        {
            if (names == null)
            {
                names = InitNames();
            }

            return names;
        }

        private static String[] InitNames()
        {
            String[] names = new String[Count];

            names[Bomb] = "Bomb";
            names[Flame] = "Flame";
            names[Disease] = "Disease";
            names[Kick] = "Kick";
            names[Speed] = "Speed";
            names[Punch] = "Punch";
            names[Grab] = "Grab";
            names[Spooger] = "Spooger";
            names[GoldFlame] = "GoldFlame";
            names[Trigger] = "Trigger";
            names[Jelly] = "Jelly";
            names[Ebola] = "Ebola";
            names[Random] = "Random";

            return names;
        }
    }
}

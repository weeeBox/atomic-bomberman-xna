using System;

namespace BombermanLive
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (BombermanGame game = new BombermanGame())
            {
                game.Run();
            }
        }
    }
#endif
}


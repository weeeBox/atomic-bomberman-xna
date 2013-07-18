using System;

namespace Bomberman
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (BmGame game = new BmGame())
            {
                game.Run();
            }
        }
    }
#endif
}


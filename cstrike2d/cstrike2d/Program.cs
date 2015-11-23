using System;

namespace cstrike2d
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CStrike2D game = new CStrike2D())
            {
                game.Run();
            }
        }
    }
#endif
}


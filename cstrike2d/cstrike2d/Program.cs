using System;

namespace CStrike2D
{
#if WINDOWS || XBOX
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            using (CStrike2D game = new CStrike2D())
            {
                game.Run();
            }
        }
    }
#endif
}


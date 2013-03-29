using System;

namespace sgd_project
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Lander game = new Lander())
            {
                game.Run();
            }
        }
    }
#endif
}


using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("UnitTesting")]

namespace RPGGame
{
    internal class Game
    {
        private static void Main()
        {
            MusicPlayer.Initialize();
            ConsoleManager.Initialize();
            TextManager.Initialize();
            ParseManager.Initialize();
            InventoryManager.Initialize();
            EntityManager.Initialize();
            GameLoop.Run();
        }
    }
}
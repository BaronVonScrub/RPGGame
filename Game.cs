using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("UnitTesting")]

namespace RPGGame
{
    class Game
    {
        static void Main()
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
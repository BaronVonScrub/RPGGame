using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("UnitTesting")]

namespace RPGGame
{
    class Game
    {
        static void Main()
        {
            MusicPlayer.Initialize();
            ConsoleHelper.Initialize();
            TextTool.Initialize();
            ParseTool.Initialize();
            InventoryManager.Initialize();
            EntityManager.Initialize();
            GameLoop.Run();
        }
    }
}
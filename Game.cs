using System;
using System.Threading;
namespace RPGGame
{
    class Game
    {
        static void Main()
        {
            ConsoleHelper.Initialize();
            TextTool.Initialize();
            ParseTool.Initialize();
            InventoryManager.Initialize();
            EntityManager.Initialize();
            MusicPlayer.Initialize();
            GameLoop.Run();
        }
    }
}
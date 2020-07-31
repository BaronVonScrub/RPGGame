using System;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("UnitTesting")]

namespace RPGGame
{
    internal class Game
    {
        private static void Main()
        {
            //All managers that require initialization recieve it, in an order respectful of dependency
            MusicPlayer.Initialize();
            ConsoleManager.Initialize();
            TextManager.Initialize();
            ParseManager.Initialize();
            InventoryManager.Initialize();
            EntityManager.Initialize();
            GameLoop.Run();                             //This initiates the interactive portion of the game.
        }
    }
}
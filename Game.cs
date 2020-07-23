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
            GameLoop.Run();
        }
    }
}
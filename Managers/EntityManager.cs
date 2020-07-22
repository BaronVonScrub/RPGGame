using System;
using System.Collections.Generic;
using System.Linq;

namespace RPGGame
{
    static class EntityManager
    {
        public static GameBoard mainBoard;
        public static Entity player;
        public static Entity merchant;
        public static Entity dummy;
        public static int inventNum = 0;

        public static void Initialize()
        {
            mainBoard = new GameBoard();

            player = new Entity("Player", new Coordinate(0, 0), (char)9787, 100, InventoryManager.GetInventory("INVENTORY"));
            merchant = new Entity("Merchant",new Coordinate(3, 0), (char)9786, 99, InventoryManager.GetInventory("MERCHANT"));
            dummy = new Entity("Dummy", new Coordinate(5, 0), (char)9786, 99,null);

            mainBoard.AddToBoard(player);
            mainBoard.AddToBoard(merchant);
            mainBoard.AddToBoard(dummy);

            Random r = new Random();
            foreach (int i in Enumerable.Range(0, 10))
            {
                mainBoard.AddToBoard(
                    new Entity(
                        "Chest",
                        new Coordinate(
                            r.Next(-10, 10),
                            r.Next(-10, 10)),
                        (char)9604, 1,
                        InventoryManager.GetInventory("CHEST" + inventNum.ToString())
                        )
                    );
                inventNum++;
            }
        }

        public static List<Entity> GetLocalEntities()
        {
            return EntityManager.mainBoard.GetFromBoard(EntityManager.player.position);
        }
    }
}

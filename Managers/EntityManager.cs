using System;
using System.Collections.Generic;
using System.Linq;
using static RPGGame.GlobalVariables;
using static RPGGame.InventoryManager;

namespace RPGGame
{
    static class EntityManager
    {
        public static Human merchant;
        public static Entity dummy;

        public static void Initialize()
        {
            MainBoard = new GameBoard();

            Player = new Human("Player", new Coordinate(0, 0), (char)9787, 100, GetInventory("INVENTORY"));
            Target = Player;
            merchant = new Human("Merchant",new Coordinate(3, 0), (char)9786, 99, GetInventory("MERCHANT"));
            dummy = new Entity("Dummy", new Coordinate(5, 0), (char)9786, 99,null);

            MainBoard.AddToBoard(Player);
            MainBoard.AddToBoard(merchant);
            MainBoard.AddToBoard(dummy);

            Random r = new Random();
            for (int i= 0;i<10;i++)
            {
                MainBoard.AddToBoard(
                    new Entity(
                        "Chest",
                        new Coordinate(
                            r.Next(-10, 10),
                            r.Next(-10, 10)),
                        (char)9604, 1,
                        GetInventory("CHEST" + InventNum.ToString())
                        )
                    );
                InventNum++;
            }
        }

        public static List<Entity> GetLocalEntities()
        {
            return MainBoard.GetFromBoard(Player.position);
        }
    }
}

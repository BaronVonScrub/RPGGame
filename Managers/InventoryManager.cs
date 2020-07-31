using System;
using System.Collections.Generic;
using static RPGGame.GlobalConstants;
using static RPGGame.GlobalVariables;
using static RPGGame.ImportExportManager;
using static RPGGame.ParseManager;
using static RPGGame.TextManager;

namespace RPGGame
{
    internal static class InventoryManager
    {
        public static int InventoryNum { get; set; } = 0;

        public static void Initialize() => ImportInventories("Inventories.dat");

        public static void TestInitialize() => ImportInventories("TestInventories.dat");

        //View inventory of provided entity
        public static void ViewInventory(Entity ent)
        {
            Target = ent;                                       //Set the target to the specified entity
            if (!InventoryIsAccessible(Target))                 //Fail if it's not accessible
            {
                WriteLine("Target inventory is not visible.");
                return;
            }

            InventoryView = true;                               //Activate inventory view

            GetCurrentInventoryList(Target).Sort(AlphabeticalByName);  //Get the inventory itemlist from the target, sorted by name.

            if (!ExternalTesting)                                       //Clear the console if not external testing
                Console.Clear();

            WriteLine(UNDERLINE + Target.Name.ToUpper() + RESET);       //Write the entity name
            if (Target == Player)
                Player.StatDisplay();                                   //Display stats if it's the player
            GoldDisplay();                                              //Display the gold

            int count = 0;                                              //Counter to allow buffer overflow

            foreach (Item item in GetCurrentInventoryList(Target).FindAll(x => (x.GetType().Name != "Gold")))   //List each non-gold item
            {
                count += 1;                                             //Increment the counter
                if (!ExternalTesting)                                   //Skip the buffer overflow if external testing
                    if (count % (Console.BufferHeight - 5) == 0)
                    {
                        Console.Write("Press enter for more...");
                        if (!InternalTesting)
                            Console.ReadKey();
                        else
                            System.Threading.Thread.Sleep(3000);        //Wait a bit, but automate if internal testing
                        Console.Clear();
                    }

                if (item.itemData.ContainsKey("amount"))                //If the item has an amount, list it, then the item
                {
                    WriteLine(item.itemData["amount"] + " " + item.Look());
                }
                else
                    WriteLine(item.Look());                             //Otherwise just list the item
            }
            WriteLine(UNDERLINE + "______________________________________________________" + RESET);
            Console.Write("Type EXIT to leave this view.");
            if (!ExternalTesting && !InternalTesting)
                Console.ReadKey();
            else
                if (!ExternalTesting)
                System.Threading.Thread.Sleep(3000);
            do
            { TextQueue.Dequeue(); }                                    //Purge the text buffer when done in the inventory
            while (TextQueue.Count != 0);
        }

        //Generate a random inventory!
        public static Inventory GenerateInv()
        {
            var r = new Random();
            int i = 0;
            int level = 0;
            do
            {
                level++;                                                //Keep incrementing the level until you roll a 1
                i = r.Next(5);
            }
            while (i != 1);
            level = Math.Min(level, ItemTable.Length);                  //limit the level by the ItemTable list length

            string inventName;
            do
            {
                InventoryNum++;
                inventName = "Inventory" + InventoryNum.ToString();     //Generate a unique inventory name by Inventory+the counter
            }
            while (Inventories.Exists(x => x.name == inventName));

            var newInv = new Inventory(inventName, new List<Item>());   //Create the new inventory

            int num = r.Next(level);

            for (int j = 0; j < num; j++)                               //Add a random number of items between 1 and level
                newInv.inventData.Add(ItemTable[r.Next(level)]);        //Of a random index between 1 and level

            Inventories.Add(newInv);                                    //Add the inventory
            return newInv;                                              //Return the inventory
        }

        //Removes an item without logging it, allows a boolean to bypass super requirements
        public static Item RemoveNoLog(bool bypass)
        {
            if (!SuperStatus && bypass == false)                //Limited to super or internal bypass
            {
                WriteLine("You do not have super access!");
                return null;
            }

            Target = GetTarget();                               //Get the target from input
            string data = Strip(Input);                         //Get the noncommand data from input

            if (GetCurrentInventoryList(Target) == null)        //If the target has no inventory
            {
                WriteLine("Inventory not found!");              //Fail
                return null;
            }
            Item remove = GetCurrentInventoryList(Target).Find(x => data.ToUpper() == x.Name.ToUpper());    //Find the item to be removed

            if (remove != null)                                 //If you find it
            {
                GetCurrentInventoryList(Target).Remove(remove); //Remove it from the inventorylist
                return remove;                                  //And return it
            }
            else
                WriteLine("Item not found!");                   //Otherwise fail
            return null;
        }

        //Takes an item from one inventory specified to another
        public static Item Grab(Entity from, Entity to)
        {

            #region Escapes
            if (!(InventoryIsAccessible(to) && InventoryIsAccessible(from)))
            {
                WriteLine("Other inventory is not accessible.");
                return null;
            }

            if (to == null || from == null)
            {
                WriteLine("Cannot trade with a null inventory!");
                return null;
            }

            if (to == from)
            {
                WriteLine("Cannot trade with yourself!");
                return null;
            }
            #endregion

            //Set the target to the specified "from" inventory
            Target = from;
            Item moveItem = RemoveNoLog(true);  //Attempt to remove and get the item via a RemoveNoLog bypass

            if (moveItem == null)               //If you failed to remove and get it
                return null;                    //Fail

            return moveItem;                    //Otherwise return it
        }

        //Shifting an item from one specified inventory to another, and exchanging the gold value of it
        public static bool Trade(Entity from, Entity to)
        {
            int value;

            //Escape
            if (from == null || to == null)
                return false;

            //Try to shift the item
            Item moveItem = Grab(from, to);

            //If you failed, fail this too. Yep, you're a damn failure.
            if (moveItem == null)
                return false;

            //If it's equipped, put it back and fail.
            if (moveItem.Equipped == true)
            {
                WriteLine("Item cannot be traded while equipped!");
                from.Inventory.inventData.Add(moveItem);
                return false;
            }

            value = moveItem.Value;                         //Get the item's value

            if (moveItem.GetType().Name == "Gold")          //If it's gold, put it back and fail.
            {
                WriteLine("Can't trade gold!");
                from.Inventory.inventData.Add(moveItem);
                return false;
            }

            if (GetGold(to) < value)                        //If the goal inventory lacks the gold for it, put it back and fail.
            {
                WriteLine("Not enough gold!");
                from.Inventory.inventData.Add(moveItem);
                return false;
            }

            from.Inventory.inventData.Add(new Gold(value)); //Add the gold value to the origin inventory
            GoldMerge(from);                                //Merge the origin's gold

            to.Inventory.inventData.Add(moveItem);          //Add the item to the goal inventory
            to.Inventory.inventData.Add(new Gold(-1 * value));  //Add a NEGATIVE valued gold item
            GoldMerge(to);                                  //Merge the goal's gold

            return true;                                    //Return a success
        }

        //Get the inventories for all the entities in a provided list.
        public static List<Inventory> GetInventories(List<Entity> entList)
        {
            var invList = new List<Inventory>();
            foreach (Entity ent in entList.FindAll(x => x.Inventory != null))
            {
                invList.Add(ent.Inventory);
            }
            return invList;
        }

        //Comparer to sort inventories by name.
        public static int AlphabeticalByName(Item a, Item b) => a.Name.CompareTo(b.Name);

        //Return the list of items of a specified entity
        public static List<Item> GetCurrentInventoryList(Entity ent)
        {
            Inventory temp = GetInventory(ent);
            if (temp == null)
                return null;
            return temp.inventData;
        }

        //Return the list of items of a specified entity by name
        public static List<Item> GetCurrentInventoryList(string invName)
        {
            Inventory temp = GetInventory(invName);
            if (temp == null)
                return null;
            return temp.inventData;
        }

        //Merge the gold items in a given entity's inventory
        public static void GoldMerge(Entity ent)
        {
            int amount = 0;

            foreach (Gold gold in GetInventory(ent).inventData.FindAll(x => x.GetType().Name == "Gold"))    //Get all the golds, sum their amounts
                amount += gold.Amount;

            ent.Inventory.inventData.RemoveAll(x => x.GetType().Name == "Gold");                            //Remove all golds

            if (amount != 0)                                                                                //If there was some value
                ent.Inventory.inventData.Add(new Gold(amount));                                             //Create a new, numerical gold item with the total value
        }

        //Return the gold held by a given entity
        public static int GetGold(Entity ent)
        {
            GoldMerge(ent);                                                                                 //Merge all the golds
            int amount = 0;
            if (!GetInventory(ent).inventData.Exists(x => x.GetType().Name == "Gold"))                      //If you have no gold, fail.
                return amount;
            amount = (GetInventory(ent).inventData.Find(x => x.GetType().Name == "Gold") as Gold).Amount;   //If you do have a gold, return its amount.
            return amount;
        }

        //Get an inventory by name
        public static Inventory GetInventory(string invName)
        {
            Inventory inv = Inventories.Find(x => x.name == invName);
            if (inv == null)
                return new Inventory(invName);
            return inv;
        }
        
        //Get an inventory by entity name.
        public static Inventory GetInventory(Entity ent)
        {
            if (ent == null)
                return null;
            Inventory inv = Inventories.Find(x => x.name == ent.Inventory.name);
            if (inv == null)
                return new Inventory(ent.Name);
            return inv;
        }

        //Check if an inventory is accessible
        public static bool InventoryIsAccessible(Entity ent)
        {
            if (ent == null)                                                                    //If the entity is null, fail.
                return false;
            if (ent.Inventory == null)                                                          //If the entity has no inventory, fail.
                return false;
            Inventory inv = GetLocalInventories().Find(x => x.name == ent.Inventory.name);      //Check if the inventory's entity is in the local square
            if (inv == null)                                                                    //If it's not, fail.
                return false;
            return true;                                                                        //Otherwise, pass!
        }

        //Gets the inventories of all entities in the player's square.
        public static List<Inventory> GetLocalInventories() => GetInventories(MainBoard.GetFromBoard(Player.position));
    }
}

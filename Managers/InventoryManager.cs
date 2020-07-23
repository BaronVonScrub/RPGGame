using System;
using System.Collections.Generic;
using static RPGGame.GlobalVariables;
using System.Text;
using static RPGGame.TextTool;

namespace RPGGame
{
    static class InventoryManager
    {
        

        public static void Initialize()
        {
            foreach (string inventoryName in ImportExportTool.GetInventoryList())                                    //For each inventory
            {
                ImportExportTool.importInventory(inventoryName);                                                     //Import the inventory
            }
        }
        public static Item RemoveNoLog()                                                                                //Remove and return the item specified in Inputfrom the Target inventory
        {
            if (SuperStatus)                                                                                      //If you have super access
            {
                Target = ParseTool.GetTarget();                                                                       //Set the inventory focus to the Input
                String data = ParseTool.Strip(Input);                                                                 //Clean the Inputdata
                Item remove = null;                                                                         //Placehold the item to be removed

                foreach (Item item in GetCurrentInventoryList())                                                  //For each item in the focused inventory
                    if (data.ToUpper() == item.itemData["name"].ToUpper() && remove == null)                //If the item is there
                    {
                        remove = item;                                                                      //Capture the item
                    }

                if (remove != null)                                                                         //If an item was caught
                {
                    GetCurrentInventoryList().Remove(remove);                                                     //Remove the item
                    return remove;                                                                          //Return the item caught
                }
                else
                {
                    WriteLine("Item not found!");
                    return null;
                }

            }
            else
            {
                WriteLine("You do not have super access!");
                return null;
            }

        }

        public static Boolean Trade(string from, string to)                                                        //Shift item from inventory to inventory for gold
        {
            if (!(InventoryIsAccessible(to)&&InventoryIsAccessible(from)))
            {
                WriteLine("Other inventory is not accessible.");
                return false;
            }

            if (to==null ||from ==null)
            {
                WriteLine("Cannot trade with a null inventory!");
                return false;
            }

            if (to == from)                                                                                 //Refuse to trade from inventory to itself
            {
                WriteLine("Cannot trade with yourself!");
                return false;
            }

            Boolean wasSuperStatus = false;                                                                       //MERCHANT SuperStatus state
            if (SuperStatus) wasSuperStatus = true;                                                                     //Grant SuperStatus access temporarily
            SuperStatus = true;

            Target = from;                                                                                  //Shift inventory focus to the seller
            Item moveItem = RemoveNoLog();                                                                       //Grab item

            if (moveItem == null)                                                                           //If the item wasn't found
            {
                if (!wasSuperStatus)                                                                              //Revoke SuperStatus access if barred
                    SuperStatus = false;
                return false;                                                                               //Return failed trade
            }

            int value = Int32.Parse(moveItem.itemData["value"]);

            if (moveItem.GetType().Name == "Gold")                                                          //If you attempted to trade gold
            {
                WriteLine("Can't trade gold!");
                GetCurrentInventoryList().Add(moveItem);                                                          //Return item
                if (!wasSuperStatus)                                                                              //Revoke SuperStatus access if barred
                    SuperStatus = false;
                return false;                                                                               //Return failed trade
            }

            if (GetGold(to) < value)                                                                       //If you don't have enough gold
            {
                WriteLine("Not enough gold!");
                GetCurrentInventoryList().Add(moveItem);                                                          //Return item
                if (!wasSuperStatus)                                                                              //Revoke SuperStatus access if barred
                    SuperStatus = false;
                return false;                                                                               //Return failed trade
            }

            GetCurrentInventoryList().Add(new Gold(value));                                                       //Add the gold to the seller
            Target = to;                                                                                    //Shift inventory focus to buyer
            GetCurrentInventoryList().Add(moveItem);                                                              //Add the item to buyer inventory
            GetCurrentInventoryList().Add(new Gold(-1 * value));                                                  //Add a negative number of gold

            if (!wasSuperStatus)
                SuperStatus = false;                                                                              //Revoke SuperStatus access if barred

            Target = from;                                                                                  //Return inventory focus to seller
            return true;                                                                                    //Return successful trade
        }

        public static List<Inventory> GetInventories(List<Entity> entList)
        {
            List<Inventory> invList = new List<Inventory>();
            foreach(Entity ent in entList)
            {
                if (ent.inventory!=null)
                    invList.Add(ent.inventory);
            }
            return invList;
        }

        public static int AlphabeticalByName(Item a, Item b)
        {
            return a.name.CompareTo(b.name);
        }

        public static List<Item> GetCurrentInventoryList()
        {
            return GetInventory(Target).inventData;
        }

        public static void GoldMerge(string invent)                                                                             //Merges all gold items in an inventory 
        {
            Queue<Gold> goldQueue = new Queue<Gold>();                                                      //A queue for each item to be processed, as removing them during the foreach breaks it
            int amount = 0;                                                                                 //Total value storage
            
            foreach (Item item in GetInventory(invent).inventData)                                                      //For each item in the inventory
            {
                if (item.GetType().Name == "Gold")                                                          //If it is gold
                {
                    goldQueue.Enqueue(item as Gold);                                                        //Add it to the processing queue
                    amount += (item as Gold).amount;                                                        //Add its value to the total
                }
            }
            if (goldQueue.Count!=0)
                do
                {
                    GetInventory(Target).inventData.Remove(goldQueue.Dequeue());                                            //Remove all golds from the inventory
                }
                while (goldQueue.Count != 0);
            GetInventory(Target).inventData.Add(new Gold(amount));                                                      //Create a new gold with the total value (integer constructor)

        }

        public static int GetGold(string invent)                                                                   //Returns the amount of gold in a given inventory
        {
            GoldMerge(invent);
            foreach (Item item in GetInventory(invent).inventData)                                                      //For each item
                if (item.GetType().Name == "Gold")                                                  //If it is gold
                    return (item as Gold).amount;                                            //Return the amount
            return 0;                                                                                       //Return zero if no gold is found
        }

        public static Inventory GetInventory(string invName)
        {
            foreach (Inventory inv in Inventories)
                if (inv.name == invName)
                    return inv;
            return new Inventory(invName);
        }

        public static Boolean InventoryIsAccessible(string invName)
        {
            foreach (Inventory inv in GetLocalInventories())
                if (inv.name == invName)
                return true;
            return false;
        }

        public static List<Inventory> GetLocalInventories()
        {
            return GetInventories(MainBoard.GetFromBoard(Player.position));
        }
    }
}

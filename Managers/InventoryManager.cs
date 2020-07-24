using System;
using System.Collections.Generic;
using static RPGGame.GlobalVariables;
using System.Text;
using static RPGGame.TextTool;
using static RPGGame.ImportExportTool;
using static RPGGame.ParseTool;

namespace RPGGame
{
    static class InventoryManager
    {
        public static void Initialize()
        {
            ImportInventories();                                                     
        }
        public static Item RemoveNoLog()                                                                                
        {
            if (!SuperStatus)
            {
                WriteLine("You do not have super access!");
                return null;
            }

            Target = GetTarget();                                                                       
            String data = Strip(Input);

            Item remove = GetCurrentInventoryList().Find(x => data.ToUpper() == x.Name.ToUpper());

            if (remove != null)                                                                         
            {
                GetCurrentInventoryList().Remove(remove);                                                     
                return remove;                                                                          
            }
            else
                WriteLine("Item not found!");
            return null;
        }

        public static Boolean Trade(Entity from, Entity to)                                                        
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

            if (to == from)                                                                                 
            {
                WriteLine("Cannot trade with yourself!");
                return false;
            }

            Boolean wasSuperStatus = false;                                                                       
            if (SuperStatus) wasSuperStatus = true;                                                                     
            SuperStatus = true;

            Target = from;                                                                                  
            Item moveItem = RemoveNoLog();                                                                       

            if (moveItem == null)                                                                           
            {
                if (!wasSuperStatus)                                                                              
                    SuperStatus = false;
                return false;                                                                               
            }

            int value = Int32.Parse(moveItem.itemData["value"]);

            if (moveItem.GetType().Name == "Gold")                                                          
            {
                WriteLine("Can't trade gold!");
                GetCurrentInventoryList().Add(moveItem);                                                          
                if (!wasSuperStatus)                                                                              
                    SuperStatus = false;
                return false;                                                                               
            }

            if (GetGold(to) < value)                                                                       
            {
                WriteLine("Not enough gold!");
                GetCurrentInventoryList().Add(moveItem);                                                          
                if (!wasSuperStatus)                                                                              
                    SuperStatus = false;
                return false;                                                                              
            }
            from.inventory.inventData.Add(new Gold(value));
            GoldMerge(from);
            to.inventory.inventData.Add(moveItem);
            to.inventory.inventData.Add(new Gold(-1 * value));
            GoldMerge(to);

            if (!wasSuperStatus)
                SuperStatus = false;                                                                                                                                                          
            return true;                                                                                    
        }

        public static List<Inventory> GetInventories(List<Entity> entList)
        {
            List<Inventory> invList = new List<Inventory>();
            foreach(Entity ent in entList.FindAll(x=> x.inventory != null))
            {
                invList.Add(ent.inventory);
            }
            return invList;
        }

        public static int AlphabeticalByName(Item a, Item b)
        {
            return a.Name.CompareTo(b.Name);
        }

        public static List<Item> GetCurrentInventoryList()
        {
            return GetInventory(Target).inventData;
        }

        public static void GoldMerge(Entity ent)                                                                             
        {                                                
            int amount = 0;                                                                                 
            
            foreach (Gold gold in GetInventory(ent).inventData.FindAll(x => x.GetType().Name == "Gold"))                                                      
                amount += gold.Amount;

            ent.inventory.inventData.RemoveAll(x => x.GetType().Name == "Gold");

            ent.inventory.inventData.Add(new Gold(amount));                                                      
        }

        public static int GetGold(Entity ent)                                                                   
        {
            GoldMerge(ent);
            int amount = 0;
            amount = (GetInventory(ent).inventData.Find(x=>x.GetType().Name == "Gold") as Gold).Amount;
            return amount;                                                                                      
        }

        public static Inventory GetInventory(string invName)
        {
            Inventory inv = Inventories.Find(x => x.name == invName);
            if (inv == null)
                return new Inventory(invName);
            return inv;
        }
        public static Inventory GetInventory(Entity ent)
        {
            if (ent == null)
                return null;
            Inventory inv = Inventories.Find(x => x.name == ent.inventory.name);
            if (inv == null)
                return new Inventory(ent.name);
            return inv;
        }


        public static Boolean InventoryIsAccessible(Entity ent)
        {
            if (ent == null)
                return false;
            Inventory inv = GetLocalInventories().Find(x => x.name == ent.inventory.name);
            if (inv == null)
                return false;
            return true;
        }

        public static List<Inventory> GetLocalInventories()
        {
            return GetInventories(MainBoard.GetFromBoard(Player.position));
        }
    }
}

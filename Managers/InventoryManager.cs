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
            foreach (string inventoryName in ImportExportTool.GetInventoryList())                                    
            {
                ImportExportTool.importInventory(inventoryName);                                                     
            }
        }
        public static Item RemoveNoLog()                                                                                
        {
            if (SuperStatus)                                                                                      
            {
                Target = ParseTool.GetTarget();                                                                       
                String data = ParseTool.Strip(Input);                                                                 
                Item remove = null;                                                                         

                foreach (Item item in GetCurrentInventoryList())                                                  
                    if (data.ToUpper() == item.itemData["name"].ToUpper() && remove == null)                
                    {
                        remove = item;                                                                      
                    }

                if (remove != null)                                                                         
                {
                    GetCurrentInventoryList().Remove(remove);                                                     
                    return remove;                                                                          
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

            GetCurrentInventoryList().Add(new Gold(value));                                                       
            Target = to;                                                                                    
            GetCurrentInventoryList().Add(moveItem);                                                              
            GetCurrentInventoryList().Add(new Gold(-1 * value));                                                  

            if (!wasSuperStatus)
                SuperStatus = false;                                                                              

            Target = from;                                                                                  
            return true;                                                                                    
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

        public static void GoldMerge(Entity ent)                                                                             
        {
            Queue<Gold> goldQueue = new Queue<Gold>();                                                      
            int amount = 0;                                                                                 
            
            foreach (Item item in GetInventory(ent).inventData)                                                      
            {
                if (item.GetType().Name == "Gold")                                                          
                {
                    goldQueue.Enqueue(item as Gold);                                                        
                    amount += (item as Gold).amount;                                                        
                }
            }
            if (goldQueue.Count!=0)
                do
                {
                    GetInventory(Target).inventData.Remove(goldQueue.Dequeue());                                            
                }
                while (goldQueue.Count != 0);
            GetInventory(Target).inventData.Add(new Gold(amount));                                                      

        }

        public static int GetGold(Entity ent)                                                                   
        {
            GoldMerge(ent);
            foreach (Item item in GetInventory(ent).inventData)                                                      
                if (item.GetType().Name == "Gold")                                                  
                    return (item as Gold).amount;                                            
            return 0;                                                                                       
        }

        public static Inventory GetInventory(string invName)
        {
            foreach (Inventory inv in Inventories)
                if (inv.name == invName)
                    return inv;
            return new Inventory(invName);
        }
        public static Inventory GetInventory(Entity ent)
        {
            if (ent != null)
                foreach (Inventory inv in Inventories)
                    if (inv.name == ent.inventory.name)
                        return inv;
            return new Inventory(ent.name);
        }


        public static Boolean InventoryIsAccessible(Entity ent)
        {
            if (ent == null)
                return false;
            foreach (Inventory inv in GetLocalInventories())
                if (inv.name == ent.inventory.name)
                return true;
            return false;
        }

        public static List<Inventory> GetLocalInventories()
        {
            return GetInventories(MainBoard.GetFromBoard(Player.position));
        }
    }
}

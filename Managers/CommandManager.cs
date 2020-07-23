using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using static RPGGame.GlobalVariables;
using static RPGGame.TextTool;
using static RPGGame.InventoryManager;

namespace RPGGame
{
    class CommandManager
    {
        public static void Unequip()
        {
            throw new NotImplementedException();
        }

        public static void Equip()
        {
            throw new NotImplementedException();
        }

        public static void Empty()
        {}

        public static void Move(MoveCommand direction)
        {
            MainBoard.entityPos[Player.position].Remove(Player);
            Player.position.x += direction.x;
            Player.position.y += direction.y;
            MainBoard.AddToBoard(Player);
       
                foreach (Entity ent in EntityManager.GetLocalEntities())
                    if (ent.name!="Player")
                        WriteLine("You see "+ent.name);

        }
        public static void Buy()                                                                                   
        {
            if (Trade(ParseTool.GetTarget(), Player))                                                       
                WriteLine("Item bought!");
            else
                WriteLine("Purchase failed!");
        }

        public static void Sell()                                                                                  
        {
            if (Trade(Player, ParseTool.GetTarget()))                                                      
                WriteLine("Item sold!");
            else
                WriteLine("Sale failed!");
        }
        public static void Look()                                                                                  
        {
            Target = ParseTool.GetTarget();
            if (!InventoryIsAccessible(Target))
            {
                WriteLine("Target inventory is not visible.");
                return;
            }

            GetCurrentInventoryList().Sort(AlphabeticalByName);

            GoldDisplay();

            foreach (Item item in GetCurrentInventoryList())
            {                                                    
                if (item.GetType().Name != "Gold")                                                          
                {
                    if (item.itemData.ContainsKey("amount"))                                                
                    {
                        WriteLine(item.itemData["amount"] + " "+item.Look());                                               
                    }
                    else
                        WriteLine(item.Look());                                                                            
                }
            }
            WriteLine(UNDERLINE + "______________________________________________________" + RESET);
        }

        public static void Examine()                                                                               
        {
            String data = ParseTool.Strip(Input);                                                                     
            Boolean found = false;                                                                          

            foreach (Item item in GetCurrentInventoryList())                                                      
            {
                if (data.ToUpper() == item.itemData["name"].ToUpper())                                        
                {
                    item.Examine();                                                                         
                    found = true;                                                                           
                }
            }
            if (!found)
                WriteLine("Item not found!");
        }

        public static void Rename()                                                                                
        {
            Target = ParseTool.GetTarget();                                                                           
            String data = ParseTool.Strip(Input);                                                                     
            Boolean found = false;                                                                          

            foreach (Item item in GetCurrentInventoryList())                                                      
            {
                if (data.Contains(item.itemData["name"]) && !found)                                          
                {
                    data = data.Replace(item.itemData["name"], "");                                          
                    data = Regex.Replace(data, "^[\\s]+|[\\s]+$", "");                                       
                    item.itemData.Remove("name");
                    item.itemData.Add("name", data);                                                        
                    item.name = data;                                                                       
                    found = true;                                                                           
                }
            }
            if (!found)
                WriteLine("Item not found!");
            else
                WriteLine("Item renamed!");
        }

        public static void GrantSuper()                                                                                 
        {
            SuperStatus = true;
            WriteLine("SuperStatus access granted!");
            WriteLine("The SuperStatus commands are ADD and REMOVE");
        }

        public static void Add()                                                                                   
        {
            if (SuperStatus)                                                                                      
            {
                Target = ParseTool.GetTarget();                                                                       
                Item newItem = ParseTool.ItemMake();                                                                  
                if (newItem != null)                                                                        
                {
                    GetCurrentInventoryList().Add(newItem);                                                       
                    WriteLine("Item added!");
                }
                else
                    WriteLine("Please include an item type, Target inventory and additional data!");
            }
            else
                WriteLine("You do not have super access!");

        }

        public static Item Remove()
        {
            Item temp = RemoveNoLog();
            WriteLine(temp.itemData["name"] + " removed!");
            return temp;
        }

        public static void Help()                                                                                  
        {
            WriteLine("The commands available to you are LOOK, EXAMINE, BUY,");
            WriteLine(" GO NORTH, GO SOUTH, GO EAST, GO WEST, SELL, RENAME,");
            WriteLine("                       HELP, QUIT");

        }

        public static void Test()                                                                                  
        {
            System.Threading.Thread.Sleep(1000);                                                   
            ConsoleHelper.Redraw();
            foreach (String test in TestCommandList)                                                      
            {
                Input = test;                                                                      
                WriteLine(Input);                                                          
                ConsoleHelper.Redraw(); ;
                System.Threading.Thread.Sleep(500);                                               
                String testCommand = ParseTool.ProcessInput(test);                                           

                WriteLine("");
                Commands[testCommand]();                                                           
                WriteLine("");
                ConsoleHelper.Redraw();
                System.Threading.Thread.Sleep(2000);                                               

            }
        }

        public static void Quit()                                                                                  
        {
            foreach (Inventory inventory in Inventories)
            {                            
                ImportExportTool.ExportInventory(inventory);                                                                 
            }
        }
    }

    struct MoveCommand
    {
        public int x;
        public int y;

        public MoveCommand(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}

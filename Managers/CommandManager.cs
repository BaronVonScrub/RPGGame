using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using static RPGGame.GlobalVariables;
using static RPGGame.TextTool;
using static RPGGame.InventoryManager;
using static RPGGame.ParseTool;
using static RPGGame.ImportExportTool;

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
       
                
            foreach (Entity ent in EntityManager.GetLocalEntities().FindAll(x => (x.name!="Player")))
                WriteLine("You see "+ent.name);

        }
        public static void Buy()                                                                                   
        {
            if (Trade(GetTarget(), Player))                                                       
                WriteLine("Item bought!");
            else
                WriteLine("Purchase failed!");
        }

        public static void Sell()                                                                                  
        {
            if (Trade(Player, GetTarget()))                                                      
                WriteLine("Item sold!");
            else
                WriteLine("Sale failed!");
        }
        public static void Look()                                                                                  
        {
            Target = GetTarget();
            if (!InventoryIsAccessible(Target))
            {
                WriteLine("Target inventory is not visible.");
                return;
            }

            GetCurrentInventoryList().Sort(AlphabeticalByName);

            GoldDisplay();

            foreach (Item item in GetCurrentInventoryList().FindAll(x => (x.GetType().Name != "Gold")))                                                 
                if (item.itemData.ContainsKey("amount"))                                                
                {
                    WriteLine(item.itemData["amount"] + " "+item.Look());                                               
                }
                else
                    WriteLine(item.Look());                                                                            
            WriteLine(UNDERLINE + "______________________________________________________" + RESET);
        }

        public static void Examine()                                                                               
        {
            String data = Strip(Input);      
            Item item = GetCurrentInventoryList().Find(x => (data == x.Name));
            if (item != null)
                item.Examine();
            else
                WriteLine("Item not found!");
        }

        public static void Rename()                                                                                
        {
            Target = GetTarget();                                                                           
            String data = Strip(Input);                                                                     

            Item item = GetCurrentInventoryList().Find(x => data.Contains(x.Name));
            if (item != null)
            {
                data = data.Replace(item.Name + " ", "");
                data = Regex.Replace(data, "^[\\s]+|[\\s]+$", "");
                item.Name = data;
                WriteLine("Item renamed!");
            }
            else
                WriteLine("Item not found!");
                
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
                Target = GetTarget();                                                                       
                Item newItem = ItemMake();                                                                  
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
            WriteLine(temp.Name + " removed!");
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
                String testCommand = ProcessInput(test);                                           

                WriteLine("");
                Commands[testCommand]();                                                           
                WriteLine("");
                ConsoleHelper.Redraw();
                System.Threading.Thread.Sleep(2000);                                               

            }
        }

        public static void Quit()                                                                                  
        {
            ExportInventories();                                                                 
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

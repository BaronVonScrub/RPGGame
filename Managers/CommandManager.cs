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
        {
            WriteLine("");
        }

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
        public static void Buy()                                                                                   //Buy an item
        {
            if (Trade(ParseTool.GetTarget(), "INVENTORY"))                                                       //Attempt trade from InputTarget to inventory
                WriteLine("Item bought!");
            else
                WriteLine("Purchase failed!");
        }

        public static void Sell()                                                                                  //Sell an item
        {
            if (Trade("INVENTORY", ParseTool.GetTarget()))                                                      //Attempt trade from inventory to InputTarget
                WriteLine("Item sold!");
            else
                WriteLine("Sale failed!");
        }
        public static void Look()                                                                                  //Look at the focused inventory
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
            {                                                    //For all items
                if (item.GetType().Name != "Gold")                                                          //That aren't gold
                {
                    if (item.itemData.ContainsKey("amount"))                                                //If they have an "amount" attribute
                    {
                        WriteLine(item.itemData["amount"] + " "+item.Look());                                               //Write it
                    }
                    else
                        WriteLine(item.Look());                                                                            //Write the item title
                }
            }
            WriteLine(UNDERLINE + "______________________________________________________" + RESET);
        }

        public static void Examine()                                                                               //View an item's information
        {
            String data = ParseTool.Strip(Input);                                                                     //Clean the Input
            Boolean found = false;                                                                          //Has the item been found?

            foreach (Item item in GetCurrentInventoryList())                                                      //For each item in the inventory
            {
                if (data.ToUpper() == item.itemData["name"].ToUpper())                                        //If it is there
                {
                    item.Examine();                                                                         //Examine it
                    found = true;                                                                           //Note that it was found
                }
            }
            if (!found)
                WriteLine("Item not found!");
        }

        public static void Rename()                                                                                //Rename an item in Inputinventory
        {
            Target = ParseTool.GetTarget();                                                                           //Set inventory focus from Input
            String data = ParseTool.Strip(Input);                                                                     //Clean the Input
            Boolean found = false;                                                                          //Has it been found?

            foreach (Item item in GetCurrentInventoryList())                                                      //For each item in the focused inventory
            {
                if (data.Contains(item.itemData["name"]) && !found)                                          //If it has a name and hasn't already been found
                {
                    data = data.Replace(item.itemData["name"], "");                                          //Remove the original name from data
                    data = Regex.Replace(data, "^[\\s]+|[\\s]+$", "");                                       //Remove preceding and trailing whitespace from data
                    item.itemData.Remove("name");
                    item.itemData.Add("name", data);                                                        //Replace the "name" attribute with the new name
                    item.name = data;                                                                       //Replace the item name
                    found = true;                                                                           //Note that it was found
                }
            }
            if (!found)
                WriteLine("Item not found!");
            else
                WriteLine("Item renamed!");
        }

        public static void GrantSuper()                                                                                 //Grant super access
        {
            SuperStatus = true;
            WriteLine("SuperStatus access granted!");
            WriteLine("The SuperStatus commands are ADD and REMOVE");
        }

        public static void Add()                                                                                   //Add a new item to the focused inventory
        {
            if (SuperStatus)                                                                                      //If you have super access
            {
                Target = ParseTool.GetTarget();                                                                       //Set the inventory focus to the Input
                Item newItem = ParseTool.ItemMake();                                                                  //Try to create a new item from Inputdata
                if (newItem != null)                                                                        //If a new item was made
                {
                    GetCurrentInventoryList().Add(newItem);                                                       //Add it to the Target inventory                                                                       //Perform a gold merge (in case new item is gold)
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

        public static void Help()                                                                                  //Print help text
        {
            WriteLine("The commands available to you are LOOK, EXAMINE, BUY,");
            WriteLine(" GO NORTH, GO SOUTH, GO EAST, GO WEST, SELL, RENAME,");
            WriteLine("                       HELP, QUIT");

        }

        public static void Test()                                                                                  //Demos all functionality                                    
        {
            System.Threading.Thread.Sleep(1000);                                                   //Waits 1s
            ConsoleHelper.Redraw();
            foreach (String test in TestCommandList)                                                      //For each command in the test list
            {
                Input = test;                                                                      //Set Inputto new command
                WriteLine(Input);                                                          //Manually display Input
                ConsoleHelper.Redraw(); ;
                System.Threading.Thread.Sleep(500);                                               //Wait to display to user
                String testCommand = ParseTool.ProcessInput(test);                                           //Extract command

                WriteLine("");
                Commands[testCommand]();                                                           //Execute command
                WriteLine("");
                ConsoleHelper.Redraw();
                System.Threading.Thread.Sleep(2000);                                               //Wait to display to user

            }
        }

        public static void Quit()                                                                                  //Saves Inventories and quits
        {
            foreach (Inventory inventory in Inventories)
            {                            //For each inventory
                ImportExportTool.ExportInventory(inventory);                                                                 //Export it
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

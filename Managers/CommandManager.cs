using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace RPGGame
{
    class CommandManager
    {
        private static MoveCommand NORTH = new MoveCommand(0, -1);
        private static MoveCommand SOUTH = new MoveCommand(0, 1);
        private static MoveCommand EAST = new MoveCommand(1, 0);
        private static MoveCommand WEST = new MoveCommand(-1, 0);

        public static Dictionary<String, Action> commands = new Dictionary<String, Action>()                        //Associates all commands with methods
                {
                    { "^$", () => Empty() },
                    { "\\bBUY\\b", () => Buy() },
                    { "\\bSELL\\b", () => Sell() },
                    { "\\bLOOK\\b", () => Look() },
                    { "\\bEXAMINE\\b", () => Examine() },
                    { "\\bRENAME\\b", () => Rename() },
                    { "^GO NORTH$|^[nN]$" , () => Move(NORTH) },
                    { "^GO SOUTH$|^[sS]$" , () => Move(SOUTH) },
                    { "^GO EAST$|^[eE]$" , () => Move(EAST) },
                    { "^GO WEST$|^[wW]$" , () => Move(WEST) },
                    { "^GIVE ME GOOD GRADES$", () => Super() },
                    { "\\bADD\\b", () => Add() },
                    { "\\bREMOVE\\b", () => Remove() },
                    { "\\bHELP\\b", () => Help() },
                    { "^QUIT$", () => Quit() },
                    { "^TEST$", () => Test() }
                };

        public static bool super = false;

        const string UNDERLINE = "\x1B[4m";                                                                  //The ANSI escape character for underline
        const string RESET = "\x1B[0m";                                                                      //The ANSI escape character to end the underline

        static List<string> types = new List<String>()                                                       //Lists possible types
            {
                "\\bWEAPON\\b",
                "\\bPOTION\\b",
                "\\bGOLD\\b",
                "\\bAMMUNITION\\b",
                "\\bARMOUR\\b",
                "\\bMISCELLANEOUS\\b",
            };

        public static void Empty()
        {
            TextTool.WriteLine("");
        }

        public static void Move(MoveCommand direction)
        {
            EntityManager.mainBoard.entityPos[EntityManager.player.position].Remove(EntityManager.player);
            EntityManager.player.position.x += direction.x;
            EntityManager.player.position.y += direction.y;
            EntityManager.mainBoard.AddToBoard(EntityManager.player);
       
                foreach (Entity ent in EntityManager.GetLocalEntities())
                    if (ent.name!="Player")
                        TextTool.WriteLine("You see "+ent.name);

        }
        public static void Buy()                                                                                   //Buy an item
        {
            if (InventoryManager.Trade(ParseTool.GetTarget(), "INVENTORY"))                                                       //Attempt trade from TextTool.TextTool.inputtarget to inventory
                TextTool.WriteLine("Item bought!");
            else
                TextTool.WriteLine("Purchase failed!");
        }

        public static void Sell()                                                                                  //Sell an item
        {
            if (InventoryManager.Trade("INVENTORY", ParseTool.GetTarget()))                                                      //Attempt trade from inventory to TextTool.TextTool.inputtarget
                TextTool.WriteLine("Item sold!");
            else
                TextTool.WriteLine("Sale failed!");
        }
        public static void Look()                                                                                  //Look at the focused inventory
        {
            InventoryManager.target = ParseTool.GetTarget();
            if (!InventoryManager.InventoryIsAccessible(InventoryManager.target))
            {
                TextTool.WriteLine("Target inventory is not visible.");
                return;
            }

            InventoryManager.GetCurrentInventoryList().Sort(InventoryManager.AlphabeticalByName);

            TextTool.GoldDisplay();

            foreach (Item item in InventoryManager.GetCurrentInventoryList())
            {                                                    //For all items
                if (item.GetType().Name != "Gold")                                                          //That aren't gold
                {
                    if (item.itemData.ContainsKey("amount"))                                                //If they have an "amount" attribute
                    {
                        TextTool.WriteLine(item.itemData["amount"] + " "+item.Look());                                               //Write it
                    }
                    else
                        TextTool.WriteLine(item.Look());                                                                            //Write the item title
                }
            }
            TextTool.WriteLine(UNDERLINE + "______________________________________________________" + RESET);
        }

        public static void Examine()                                                                               //View an item's information
        {
            String data = ParseTool.Strip(TextTool.input);                                                                     //Clean the TextTool.input
            Boolean found = false;                                                                          //Has the item been found?

            foreach (Item item in InventoryManager.GetCurrentInventoryList())                                                      //For each item in the inventory
            {
                if (data.ToUpper() == item.itemData["name"].ToUpper())                                        //If it is there
                {
                    item.Examine();                                                                         //Examine it
                    found = true;                                                                           //Note that it was found
                }
            }
            if (!found)
                TextTool.WriteLine("Item not found!");
        }

        public static void Rename()                                                                                //Rename an item in TextTool.TextTool.inputinventory
        {
            InventoryManager.target = ParseTool.GetTarget();                                                                           //Set inventory focus from TextTool.input
            String data = ParseTool.Strip(TextTool.input);                                                                     //Clean the TextTool.input
            Boolean found = false;                                                                          //Has it been found?

            foreach (Item item in InventoryManager.GetCurrentInventoryList())                                                      //For each item in the focused inventory
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
                TextTool.WriteLine("Item not found!");
            else
                TextTool.WriteLine("Item renamed!");
        }

        public static void Super()                                                                                 //Grant super access
        {
            super = true;
            TextTool.WriteLine("Super access granted!");
            TextTool.WriteLine("The Super commands are ADD and REMOVE");
        }

        public static void Add()                                                                                   //Add a new item to the focused inventory
        {
            if (super)                                                                                      //If you have super access
            {
                InventoryManager.target = ParseTool.GetTarget();                                                                       //Set the inventory focus to the TextTool.input
                Item newItem = ParseTool.ItemMake();                                                                  //Try to create a new item from TextTool.TextTool.inputdata
                if (newItem != null)                                                                        //If a new item was made
                {
                    InventoryManager.GetCurrentInventoryList().Add(newItem);                                                       //Add it to the InventoryManager.target inventory                                                                       //Perform a gold merge (in case new item is gold)
                    TextTool.WriteLine("Item added!");
                }
                else
                    TextTool.WriteLine("Please include an item type, InventoryManager.target inventory and additional data!");
            }
            else
                TextTool.WriteLine("You do not have super access!");

        }

        public static Item Remove()
        {
            Item temp = InventoryManager.RemoveNoLog();
            TextTool.WriteLine(temp.itemData["name"] + " removed!");
            return temp;
        }

        public static void Help()                                                                                  //Print help text
        {
            TextTool.WriteLine("The commands available to you are LOOK, EXAMINE, BUY,");
            TextTool.WriteLine(" GO NORTH, GO SOUTH, GO EAST, GO WEST, SELL, RENAME,");
            TextTool.WriteLine("                       HELP, QUIT");

        }

        public static void Test()                                                                                  //Demos all functionality                                    
        {
            System.Threading.Thread.Sleep(1000);                                                   //Waits 1s
            ConsoleHelper.Redraw();
            foreach (String test in TestTool.testList)                                                      //For each command in the test list
            {
                TextTool.input = test;                                                                      //Set TextTool.TextTool.inputto new command
                TextTool.WriteLine(TextTool.input);                                                          //Manually display TextTool.input
                ConsoleHelper.Redraw(); ;
                System.Threading.Thread.Sleep(500);                                               //Wait to display to user
                String testCommand = ParseTool.ProcessInput(test);                                           //Extract command

                TextTool.WriteLine("");
                ParseTool.commands[testCommand]();                                                           //Execute command
                TextTool.WriteLine("");
                ConsoleHelper.Redraw();
                System.Threading.Thread.Sleep(2000);                                               //Wait to display to user

            }
        }

        public static void Quit()                                                                                  //Saves InventoryManager.inventories and quits
        {
            foreach (Inventory inventory in InventoryManager.inventories)
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

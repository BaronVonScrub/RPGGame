using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RPGGame
{
    static class ParseTool
    {
        public static String keyList = "";                                                                   //Used for a regex to find all keywords

        public static Dictionary<String, Action> commands = CommandManager.commands;

        static List<string> types = new List<String>()                                                       //Lists possible types
            {
                "\\bWEAPON\\b",
                "\\bPOTION\\b",
                "\\bGOLD\\b",
                "\\bAMMUNITION\\b",
                "\\bARMOUR\\b",
                "\\bMISCELLANEOUS\\b",
            };

        public static void Initialize()
        {
            KeyListCreate();
        }

        public static String ProcessInput(string inp)                                                            //Extract commands from input 
        {
            InventoryManager.target = GetTarget();                                                                            //Change inventory focus based on input
            foreach (KeyValuePair<String, Action> command in commands)
            {                                      //Check input against each command
                Match match = Regex.Match(inp, command.Key);
                if (match.Success)
                    return command.Key;                                                                      //Breaks loop - note that earliest command in array prioritised
            }
            return "";                                                                                       //Return empty if no command found
        }

        public static String GetTarget()                                                                           //Get focused inventory from input 
        {
            String tempTarget = InventoryManager.target;                                                                              //Default to Inventory if no previous focus                                                                      //Else default to preveious

            foreach (Inventory inventory in InventoryManager.GetLocalInventories())                             //For each inventory
            {
                if (Regex.Match(TextTool.input, "\\b"+inventory.name+"\\b").Success)                                              //If one is found
                {
                    tempTarget = inventory.name;
                    return tempTarget;                                                                      //return it
                }
            }
            return tempTarget;                                                                              //Otherwise return the default
        }

        public static String GetItemType()                                                                             //Get type from input 
        {
            String tempType = null;                                                                          //Default to null

            foreach (String type in types)                                                                  //For each possible type
            {
                if (Regex.Match(TextTool.input, type).Success)
                {
                    tempType = StripRegex(type);
                    return tempType;                                                                        //Return it if found
                }
            }
            return tempType;                                                                                //Else return default (Null)
        }

        public static String GetItemType(String indata)                                                                             //Get type from input 
        {
            String tempType = null;                                                                          //Default to null

            foreach (String type in types)                                                                  //For each possible type
            {
                if (Regex.Match(indata, type).Success)
                {
                    tempType = StripRegex(type);
                    return tempType;                                                                        //Return it if found
                }
            }
            return tempType;                                                                                //Else return default (Null)
        }

        public static String Strip(string indata)                                                                  //Cleans non-keyword data in input 
        {
            String data = Regex.Replace(indata, ParseTool.keyList, "");                                                //Remove all keywords
            data = Regex.Replace(data, "^\\s*|\\s*$", "");                                                  //Remove preceding and trailing whitespace
            return data;                                                                                    //Return what is left
        }

        public static void KeyListCreate()
        {
            foreach (KeyValuePair<String, Action> command in commands)
                keyList += command.Key + "|";
            foreach (String itemType in types)
                keyList += itemType + "|";
            foreach (Inventory inventory in InventoryManager.inventories)
                keyList += inventory.name + "|";
            keyList = keyList.Substring(0, keyList.Length - 1);
        }

        public static Item ItemMake()                                                                              //Create item from input
        {
            string type = GetItemType();                                                                        //Get the item type
            String data = Strip(TextTool.input);                                                                     //Clean the input
            if (data != "")                                                                                   //If any data remains
                return type switch                                                                          //Create an item based on type
                {
                    "WEAPON" => new Weapon(data),
                    "POTION" => new Potion(data),
                    "GOLD" => new Gold(data),
                    "AMMUNITION" => new Ammunition(data),
                    "ARMOUR" => new Armour(data),
                    _ => new Miscellaneous(data)
                };
            else
                return null;                                                                                //Return null if no data
        }

        public static Item ItemMake(String indata)                                                                              //Create item from input
        {
            string type = GetItemType(indata);                                                                        //Get the item type
            String data = Strip(indata);                                                                     //Clean the input
            if (data != "")                                                                                   //If any data remains
                return type switch                                                                          //Create an item based on type
                {
                    "WEAPON" => new Weapon(data),
                    "POTION" => new Potion(data),
                    "GOLD" => new Gold(data),
                    "AMMUNITION" => new Ammunition(data),
                    "ARMOUR" => new Armour(data),
                    _ => new Miscellaneous(data)
                };
            else
                return null;                                                                                //Return null if no data
        }

        public static String StripRegex(String inp)
        {
            return inp.Replace("\\b", "");
        } 
    }
}

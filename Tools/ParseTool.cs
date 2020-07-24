using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using static RPGGame.InventoryManager;
using static RPGGame.GlobalVariables;
using static RPGGame.EntityManager;
using System.Reflection;

namespace RPGGame
{
    static class ParseTool
    {
        public static void Initialize()
        {
            KeyListCreate();
        }

        public static String ProcessInput(string inp)                                                            
        {
            Target = GetTarget();

            String command = Commands.Keys.ToList().Find(x => Regex.Match(inp, x).Success);
            if (command == null)
                return "";
            return command;                                                                                       
        }

        public static Entity GetTarget()                                                                           
        {
            Entity tempTarget = MainBoard.GetFromBoard(Player.position).Find(x => Regex.Match(Input, "\\b" + x.Name + "\\b").Success);
            if (tempTarget == null)
                return Target;
            return tempTarget;                                                                              
        }

        public static String GetItemType()                                                                             
        {
            String tempType = null;
            tempType = Types.Find(x => Regex.Match(Input, "\\b" + x + "\\b").Success);
            return tempType;                                                                                
        }

        public static String GetItemType(String indata)                                                                             
        {
            String tempType = null;
            tempType = Types.Find(x => Regex.Match(indata, "\\b" + x + "\\b").Success);
            return tempType;                                                                                
        }

        public static String Strip(string indata)                                                                  
        {
            String data = Regex.Replace(indata, KeyList, "");                                                
            data = Regex.Replace(data, "^\\s*|\\s*$", "");                                                  
            return data;                                                                                    
        }

        static string FirstUpperOnly(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1).ToLower();
        }

        public static void KeyListCreate()
        {
            foreach (KeyValuePair<String, Action> command in Commands)
                KeyList += command.Key + "|";
            foreach (String itemType in Types)
                KeyList += itemType + "|";
            foreach (Inventory inventory in Inventories)
                KeyList += inventory.name + "|";
            KeyList = KeyList[0..^1];
        }

        public static Entity EntityFactory(string[] entData)
        {
            Inventory inv = null;
            if (entData[5] != "Null")
                inv = GetInventory(entData[5]);

            return EntityFactoryReflection(entData[1], entData[0], new Coordinate(entData[4].Split(" ")), (char)Int32.Parse(entData[2]), Int32.Parse(entData[3]), inv);
        }

        public static Item ItemMake()                                                                              
        {
            return ItemMake(Input);                                                                             
        }

        public static Item ItemMake(String indata)                                                                              
        {
            string type = FirstUpperOnly(GetItemType(indata));                                                                        
            String data = Strip(indata);
            if (data != "")
                return ItemFactoryReflection(type, indata);
            else
                return null;                                                                                
        }

        public static dynamic ItemFactoryReflection(string entType, string indata)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            var currentType = currentAssembly.GetTypes().SingleOrDefault(t => t.Name == entType);
            return Activator.CreateInstance(currentType, indata);
        }
    }
}

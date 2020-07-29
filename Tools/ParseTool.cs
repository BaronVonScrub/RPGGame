using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using static RPGGame.InventoryManager;
using static RPGGame.GlobalVariables;
using static RPGGame.ConstantVariables;
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

        public static string ProcessInput(string inp)                                                            
        {
            Target = GetTarget();

            string command = Commands.Keys.ToList().Find(x => Regex.Match(inp, x).Success);
            if (command == null)
                return "";
            return command;                                                                                       
        }

        public static Entity GetTarget(Entity ent, string inp ,GameBoard board)                                                                           
        {
            List<Entity> localEnts = board.GetFromBoard(ent.position);
            Entity tempTarget = board.GetFromBoard(ent.position).Find(x => Regex.Match(inp, "\\b" + x.Name + "\\b").Success);
            if (tempTarget == null)
                return Target;
            return tempTarget;                                                                              
        }

        public static Entity GetTarget()
        {
            return GetTarget(Player, Input, MainBoard);
        }

        public static Entity GetTarget(string inp)
        {
            return GetTarget(Player, inp, MainBoard);
        }

        public static string GetItemType()                                                                             
        {
            string tempType = null;
            tempType = Types.Find(x => Regex.Match(Input, "\\b" + x + "\\b").Success);
            return tempType;                                                                                
        }

        public static string GetItemType(String indata)                                                                             
        {
            string tempType = null;
            tempType = Types.Find(x => Regex.Match(indata, "\\b" + x + "\\b").Success);
            return tempType;                                                                                
        }

        public static string Strip(string indata)                                                                  
        {
            string data = Regex.Replace(indata, KeyList, "");                                                
            data = Regex.Replace(data, "^\\s*|\\s*$", "");                                                  
            return data;                                                                                    
        }

        public static string FirstUpperOnly(string s)
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

        public static dynamic ItemCreate(ItemData inData)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            var currentType = currentAssembly.GetTypes().SingleOrDefault(t => t.Name == inData.type);
            if (currentType == null)
                return null;
            return Activator.CreateInstance(currentType,inData.data);
        }

        public static dynamic ItemCreate()
        {
            ItemData inData = new ItemData(Input);
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            var currentType = currentAssembly.GetTypes().SingleOrDefault(t => t.Name == inData.type);
            if (currentType == null)
                return null;
            return Activator.CreateInstance(currentType, inData.data);
        }
    }
}

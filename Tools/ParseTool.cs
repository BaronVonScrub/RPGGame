using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using static RPGGame.GlobalVariables;

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
            Entity tempTarget = MainBoard.GetFromBoard(Player.position).Find(x => Regex.Match(Input, "\\b" + x.name + "\\b").Success);
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

        public static Item ItemMake()                                                                              
        {
            string type = GetItemType();                                                                        
            String data = Strip(Input);                                                                
            if (data != "")                                                                                   
                return type switch                                                                          
                {
                    "WEAPON" => new Weapon(data),
                    "POTION" => new Potion(data),
                    "GOLD" => new Gold(data),
                    "AMMUNITION" => new Ammunition(data),
                    "RING" => new Ring(data),
                    "ARMOUR" => new Armour(data),
                    _ => new Miscellaneous(data)
                };
            else
                return null;                                                                                
        }

        public static Item ItemMake(String indata)                                                                              
        {
            string type = GetItemType(indata);                                                                        
            String data = Strip(indata);                                                                     
            if (data != "")                                                                                   
                return type switch                                                                          
                {
                    "WEAPON" => new Weapon(data),
                    "POTION" => new Potion(data),
                    "GOLD" => new Gold(data),
                    "AMMUNITION" => new Ammunition(data),
                    "RING" => new Ring(data),
                    "ARMOUR" => new Armour(data),
                    _ => new Miscellaneous(data)
                };
            else
                return null;                                                                                
        }
    }
}

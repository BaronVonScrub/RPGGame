using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static RPGGame.GlobalVariables;
using static RPGGame.InventoryManager;

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
            foreach (KeyValuePair<String, Action> command in Commands)
            {                                      
                Match match = Regex.Match(inp, command.Key);
                if (match.Success)
                    return command.Key;                                                                      
            }
            return "";                                                                                       
        }

        public static Entity GetTarget()                                                                           
        {
            Entity tempTarget = Target;

            foreach (Entity ent in MainBoard.GetFromBoard(Player.position))                             
            {
                if (Regex.Match(Input, "\\b"+ent.name+"\\b").Success)                                              
                {
                    tempTarget = ent;
                    return tempTarget;                                                                      
                }
            }
            return tempTarget;                                                                              
        }

        public static String GetItemType()                                                                             
        {
            String tempType = null;                                                                          

            foreach (String type in Types)                                                                  
            {
                if (Regex.Match(Input, type).Success)
                {
                    tempType = StripRegex(type);
                    return tempType;                                                                        
                }
            }
            return tempType;                                                                                
        }

        public static String GetItemType(String indata)                                                                             
        {
            String tempType = null;                                                                          

            foreach (String type in Types)                                                                  
            {
                if (Regex.Match(indata, type).Success)
                {
                    tempType = StripRegex(type);
                    return tempType;                                                                        
                }
            }
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
            KeyList = KeyList.Substring(0, KeyList.Length - 1);
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

        public static String StripRegex(String inp)
        {
            return inp.Replace("\\b", "");
        } 
    }
}

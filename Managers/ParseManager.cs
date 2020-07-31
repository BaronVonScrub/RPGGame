using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using static RPGGame.GlobalConstants;
using static RPGGame.GlobalVariables;
using static RPGGame.TextManager;

namespace RPGGame
{
    internal static class ParseManager
    {
        //Initialization creates the Regex for use in stripping command data from input. This method, rather than hard coding, allows me to easily expand the list of commands
        //without having to manually update the keylist.
        public static void Initialize() => KeyListCreate();

        //Processes input to find the command stated
        public static string ProcessInput(string inp)
        {
            WriteLine(inp);                                                                 //Write the input down
            Target = GetTarget();                                                           //Find an entity target from the input

            string command = Commands.Keys.ToList().Find(x => Regex.Match(inp, x).Success); //Find the command listed in the commandlist.
            if (command == null)                                                            //Return blank if there is none there.
                return "";
            return command;                                                                 //Otherwise return the found command
        }

        //Get the entity specified at the specified entity's position, stated in the input string, within the specified gameboard
        public static Entity GetTarget(Entity ent, string inp, GameBoard board)
        {
            List<Entity> localEnts = board.GetFromBoard(ent.position);                      //Get the list of local entities
                                                                                            //Find a name in that list that is found in the input
            Entity tempTarget = board.GetFromBoard(ent.position).Find(x => Regex.Match(inp.ToUpper(), "\\b" + x.Name.ToUpper() + "\\b").Success);
            if (Target == null)                                                             //Default Target to player
                Target = Player;
            if (tempTarget == null)                                                         //If the search didn't find anything
                return Target;                                                              //Return the default current Target
            return tempTarget;                                                              //Otherwise return the found entity
        }

        //GetTarget with no commands uses Player, Input, Mainboard in the more extensive GetTarget method
        public static Entity GetTarget()
        {
            if (Player == null || MainBoard == null)                                        //Fail if Player or Board are null.
                return null;
            return GetTarget(Player, Input, MainBoard);                                     //Otherwise, return target based on them and Input.
        }

        //Get the type of item listed in Input
        public static string GetItemType()
        {
            return GetItemType(Input);
        }

        //Get the type of item listed in the input string
        public static string GetItemType(string indata)
        {
            string tempType = null;
            tempType = Types.Find(x => Regex.Match(indata, "\\b" + x + "\\b").Success);
            return tempType;
        }

        //Strip all command information from a string
        public static string Strip(string indata)
        {
            string data = Regex.Replace(indata, KeyList, "");           //Replace all keywords with nothing
            data = Regex.Replace(data, "^\\s*|\\s*$", "");              //Strip the whitespace
            return data;                                                //Return what remains
        }

        //Returns a string based on the substring, but with only the first letter capitalized.
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
        
        //Create the keylist
        public static void KeyListCreate()
        {
            foreach (KeyValuePair<string, Action> command in Commands)
                KeyList += command.Key + "|";                               //Add all command keywords
            foreach (string itemType in Types)
                KeyList += itemType + "|";                                  //Add all item types
            foreach (Inventory inventory in Inventories)                    //Add all inventories
                KeyList += inventory.name + "|";
            KeyList = KeyList[0..^1];                                       //Remove the last character (trailing |)
        }

        //Create and return an item via reflection from an ItemData struct
        public static dynamic ItemCreate(ItemData inData)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();                                      //Get current assembly
            Type currentType = currentAssembly.GetTypes().SingleOrDefault(t => t.Name == inData.type);  //Find the provided type in the list of assembly's types
            if (currentType == null)                                                                    //If you don't find it, fail
                return null;
            return Activator.CreateInstance(currentType, inData.data);                                  //Otherwise, create the instance
        }

        //Create and return an item via reflection from Input
        public static dynamic ItemCreate()
        {
            var inData = new ItemData(Input);                                                           //Create a new ItemData struct from input
            var currentAssembly = Assembly.GetExecutingAssembly();                                      //Get current assembly
            Type currentType = currentAssembly.GetTypes().SingleOrDefault(t => t.Name == inData.type);  //Find the provided type in the list of assembly's types
            if (currentType == null)                                                                    //If you don't find it, fail.
                return null;
            return Activator.CreateInstance(currentType, inData.data);                                  //Otherwise, create the instance
        }
    }
}
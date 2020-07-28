using System;
using System.Collections.Generic;

namespace RPGGame
{
    class GlobalVariables
    {
        //Dynamically change throughout the program, want easily accessible to whoever needs them.

        public static Entity Player { get; set; } = null;                                   //Contains the entities that is the player

        public static GameBoard MainBoard { get; set; } = null;                             //Contains the current gameboard

        public static string Input { get; set; } = "";                                      //Contains the latest console input

        public static bool SuperStatus { get; set; } = false;                               //Contains the admin status

        public static string CurrentCommand { get; set; } = "";                             //Contains the current command

        public static Entity Target { get; set; } = null;                                   //Contains the current targeted entity

        public static List<Inventory> Inventories { get; set; } = new List<Inventory>();    //Contains a list of all the inventories

        public static string KeyList { get; set; } = "";                                    //Contains a list of all commands in Regex (for stripping)

        public static Queue<Line> TextQueue { get; set; } = new Queue<Line>();              //Contains a list of recent console input/output

        public static Boolean Mute { get; set; } = false;                                   //Contains the mute state of the game
    }
}

using System;
using System.Collections.Generic;

namespace RPGGame
{
    class GlobalVariables
    {
        //Dynamically change throughout the program, want easily accessible to whoever needs them.

        public static Entity Player { get; set; }
        public static GameBoard MainBoard { get; set; }
        public static string Input { get; set; }
        public static bool SuperStatus { get; set; }
        public static string CurrentCommand { get; set; }
        public static Entity Target { get; set; }
        public static List<Inventory> Inventories { get; set; }
        public static string KeyList { get; set; }
        public static Queue<Line> TextQueue { get; set; }
        public static Boolean Mute { get; set; }
    }
}

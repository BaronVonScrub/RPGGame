using System;
using System.Collections.Generic;
using System.Text;
using static RPGGame.CommandManager;

namespace RPGGame
{
    static class GlobalVariables
    {
        public const int padding = 15;
        public const char LeftTopCornerBorder = (char)9556;
        public const char HorizontalBorder = (char)9552;
        public const char RightTopCornerBorder = (char)9559;
        public const char VerticalBorder = (char)9553;
        public const char LeftBottomCornerBorder = (char)9562;
        public const char RightBottomCornerBorder = (char)9565;
        public const int viewDistanceWidth = 5;
        public const int viewDistanceHeight = 5;
        public const string UNDERLINE = "\x1B[4m";                                                                  //The ANSI escape character for underline
        public const string RESET = "\x1B[0m";                                                                      //The ANSI escape character to end the underline

        private static Entity player;
        private static GameBoard mainBoard;
        private static int inventNum;
        private static String input = "";
        private static readonly String attFinder = "(\\S+:[\\w\\s]+)(?=\\s|$)";                                          //The Regex format to parse attributes
        private static bool superStatus = false;
        private static String target = "INVENTORY";
        private static List<Inventory> inventories = new List<Inventory>();
        private static Queue<Line> textQueue;
        private static String keyList = "";                                                                   //Used for a regex to find all keywords

        public static int InventNum { get => inventNum; set => inventNum = value; }
        internal static Entity Player { get => player; set => player = value; }
        internal static GameBoard MainBoard { get => mainBoard; set => mainBoard = value; }
        public static Dictionary<string, Action> Commands { get => commands; set => commands = value; }
        public static string Input { get => input; set => input = value; }
        public static List<string> Types { get => types; set => types = value; }
        public static String CurrentCommand { get; set; }                                                                        //Current valid command
        public static string AttFinder => attFinder;
        public static bool SuperStatus { get => superStatus; set => superStatus = value; }
        public static string Target { get => target; set => target = value; }
        internal static List<Inventory> Inventories { get => inventories; set => inventories = value; }
        public static string KeyList { get => keyList; set => keyList = value; }
        internal static Queue<Line> TextQueue { get => textQueue; set => textQueue = value; }
        public static List<string> TestCommandList { get => testCommandList; set => testCommandList = value; }

        private static Dictionary<String, Action> commands = new Dictionary<String, Action>()                        //Associates all commands with methods
                {
                    { "^$", () => Empty() },
                    { "\\bBUY\\b", () => Buy() },
                    { "\\bSELL\\b", () => Sell() },
                    { "\\bLOOK\\b", () => Look() },
                    { "\\bEXAMINE\\b", () => Examine() },
                    { "\\bRENAME\\b", () => Rename() },
                    { "\\bEQUIP\\b", () => Equip() },
                    { "\\bUNEQUIP\\b", () => Unequip() },
                    { "^GO NORTH$|^[nN]$" , () => Move(NORTH) },
                    { "^GO SOUTH$|^[sS]$" , () => Move(SOUTH) },
                    { "^GO EAST$|^[eE]$" , () => Move(EAST) },
                    { "^GO WEST$|^[wW]$" , () => Move(WEST) },
                    { "^GIVE ME GOOD GRADES$", () => GrantSuper() },
                    { "\\bADD\\b", () => Add() },
                    { "\\bREMOVE\\b", () => Remove() },
                    { "\\bHELP\\b", () => Help() },
                    { "^QUIT$", () => Quit() },
                    { "^TEST$", () => Test() }
                };

        static List<string> types = new List<String>()                                                       //Lists possible types
            {
                "\\bWEAPON\\b",
                "\\bPOTION\\b",
                "\\bGOLD\\b",
                "\\bAMMUNITION\\b",
                "\\bARMOUR\\b",
                "\\bRING\\b",
                "\\bMISCELLANEOUS\\b",
            };

        private static MoveCommand NORTH = new MoveCommand(0, -1);
        private static MoveCommand SOUTH = new MoveCommand(0, 1);
        private static MoveCommand EAST = new MoveCommand(1, 0);
        private static MoveCommand WEST = new MoveCommand(-1, 0);

        private static List<string> testCommandList = new List<string>                                                        //List of test commands
            {
            "HELP",
            "LOOK INVENTORY",
            "LOOK MERCHANT",
            "EXAMINE Axe",
            "BUY Axe",
            "LOOK INVENTORY",
            "LOOK MERCHANT",
            "SELL Axe",
            "LOOK INVENTORY",
            "LOOK MERCHANT",
            "RENAME INVENTORY Bow Longbow",
            "LOOK INVENTORY",
            "EXAMINE Longbow",
            "EXAMINE Bow",
            "EXAMINE MERCHANT Axe",
            "GIVE ME GOOD GRADES",
            "ADD MERCHANT WEAPON name:Rapier attackModifier:5 value:35",
            "LOOK MERCHANT",
            "EXAMINE Rapier",
            "REMOVE Rapier",
            "LOOK MERCHANT",
            "QUIT"
            };
    }
}
struct Coordinate
{
    public int x;
    public int y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
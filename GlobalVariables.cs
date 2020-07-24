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
        public const string UNDERLINE = "\x1B[4m";                                                                  
        public const string RESET = "\x1B[0m";
        public const char Ver = (char)9474;
        public const char Hor = (char)9472;
        public const char TopL = (char)9484;
        public const char TopR = (char)9488;
        public const char BotL = (char)9492;
        public const char BotR = (char)9496;
        public const char TeeL = (char)9508;
        public const char TeeR = (char)9500;
        public const char TeeU = (char)9524;
        public const char TeeD = (char)9516;
        public const char Cross = (char)9532;


        private static Entity player;
        private static GameBoard mainBoard;
        private static int inventNum;
        private static String input = "";
        private static readonly String attFinder = "(\\S+:[\\w\\s]+)(?=\\s|$)";                                          
        private static bool superStatus = false;
        private static Entity target;
        private static List<Inventory> inventories = new List<Inventory>();
        private static Queue<Line> textQueue = new Queue<Line>();
        private static String keyList = "";                                                                   

        public static int InventNum { get => inventNum; set => inventNum = value; }
        internal static Entity Player { get => player; set => player = value; }
        internal static GameBoard MainBoard { get => mainBoard; set => mainBoard = value; }
        public static Dictionary<string, Action> Commands { get => commands; set => commands = value; }
        public static string Input { get => input; set => input = value; }
        public static List<string> Types { get => types; set => types = value; }
        public static String CurrentCommand { get; set; }                                                                        
        public static string AttFinder => attFinder;
        public static bool SuperStatus { get => superStatus; set => superStatus = value; }
        public static Entity Target { get => target; set => target = value; }
        internal static List<Inventory> Inventories { get => inventories; set => inventories = value; }
        public static string KeyList { get => keyList; set => keyList = value; }
        internal static Queue<Line> TextQueue { get => textQueue; set => textQueue = value; }
        public static List<string> TestCommandList { get => testCommandList; set => testCommandList = value; }

        private static Dictionary<String, Action> commands = new Dictionary<String, Action>()                        
                {
                    { "^$", () => Empty() },
                    { "\\bBUY\\b", () => Buy() },
                    { "\\bSELL\\b", () => Sell() },
                    { "\\bLOOK\\b", () => Look() },
                    { "\\bEXAMINE\\b", () => Examine() },
                    { "\\bRENAME\\b", () => Rename() },
                    { "\\bEQUIP\\b", () => Equip() },
                    { "\\bUNEQUIP\\b", () => Unequip() },
                    { "\\bTAKE\\b", () => Take() },
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

        static List<string> types = new List<String>()                                                       
            {
                "WEAPON",
                "POTION",
                "GOLD",
                "AMMUNITION",
                "ARMOUR",
                "RING",
                "MISCELLANEOUS",
            };

        private static MoveCommand NORTH = new MoveCommand(0, -1);
        private static MoveCommand SOUTH = new MoveCommand(0, 1);
        private static MoveCommand EAST = new MoveCommand(1, 0);
        private static MoveCommand WEST = new MoveCommand(-1, 0);

        private static List<string> testCommandList = new List<string>                                                        
            {
            "HELP",
            "LOOK Player",
            "GO EAST",
            "GO EAST",
            "E",
            "LOOK Merchant",
            "EXAMINE Axe",
            "BUY Axe",
            "LOOK Player",
            "LOOK Merchant",
            "SELL Axe",
            "LOOK Player",
            "LOOK Merchant",
            "RENAME Player Bow Longbow",
            "LOOK Player",
            "EXAMINE Longbow",
            "EXAMINE Bow",
            "EXAMINE Player Axe",
            "GIVE ME GOOD GRADES",
            "ADD Merchant WEAPON name:Rapier attackModifier:5 value:35",
            "LOOK Merchant",
            "EXAMINE Rapier",
            "REMOVE Rapier",
            "LOOK Merchant",
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

    public Coordinate(String[] inp)
    {
        this.x = Int32.Parse(inp[0]);
        this.y = Int32.Parse(inp[1]);
    }
}
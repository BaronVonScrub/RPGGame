using System;
using System.Collections.Generic;
using static RPGGame.CommandManager;

namespace RPGGame
{
    static class ConstantVariables
    {
        //Never altered during runtime

        public static List<string> Types => types;
        public static string AttFinder => attFinder;
        public static Dictionary<string, Action> Commands => commands;
        public static List<string> TestCommandList => testCommandList;

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

        private const string attFinder = "(\\S+:[\\w\\s]+)(?=\\s|$)";

        private readonly static Dictionary<String, Action> commands = new Dictionary<String, Action>()                        
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
                    { "\\bMUTE\\b", () => MuteToggle() },
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

        static readonly List<string> types = new List<String>()                                                       
            {
                "WEAPON",
                "POTION",
                "GOLD",
                "AMMUNITION",
                "ARMOUR",
                "RING",
                "MISCELLANEOUS",
            };

        private readonly static MoveCommand NORTH = new MoveCommand(0, -1);
        private readonly static MoveCommand SOUTH = new MoveCommand(0, 1);
        private readonly static MoveCommand EAST = new MoveCommand(1, 0);
        private readonly static MoveCommand WEST = new MoveCommand(-1, 0);

        private readonly static List<string> testCommandList = new List<string>                                                        
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
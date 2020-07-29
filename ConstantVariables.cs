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

        public const char LTPath = (char)9556;
        public const char HPath = (char)9552;
        public const char RTPath = (char)9559;
        public const char VPath = (char)9553;
        public const char LBPath = (char)9562;
        public const char RBPath = (char)9565;
        public const char XPath = (char)9580;
        public const char TTPath = (char)9577;
        public const char TBPath = (char)9574;
        public const char TLPath = (char)9571;
        public const char TRPath = (char)9568;

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

        public static Weapon Fist = new Weapon("attackModifier:2 damageModifier:1 maxRange:1 minRange:1 slotsNeeded:1 value:0 name:Fist equipped:true");

        private const string attFinder = "(\\S+:[\\w\\s]+)(?=\\s|$)";

        public static readonly int MaxHealth = 0;
        public static readonly int CurrHealth = 1;
        public static readonly int BaseArmour = 2;
        public static readonly int Speed = 3;
        public static readonly int Distance = 4;

        private readonly static Dictionary<String, Action> commands = new Dictionary<String, Action>()                        
                {
                    { "^$", () => Empty() },
                    { "\\bBUY\\b", () => Buy() },
                    { "\\bSELL\\b", () => Sell() },
                    { "\\bME\\b", () => LookAtMe() },
                    { "\\bLOOK\\b|\\bTALK\\b", () => Look() },
                    { "\\bTRADE\\b", () => TradeWith() },
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


        public static readonly Item[] ItemTable = new Item[] {
            new Gold(1),
            new Gold(2),
            new Gold(5),
            new Gold(10),
            new Ammunition("amount:5 value:1 name:Rock"),
            new Weapon("name:Shiv equipped:false value:5 slotsNeeded:1 minRange:1 maxRange:1 damageModifier:2 attackModifier:2"),
            new Armour("name:Rags equipped:false value:5 armourModifier:0 defenceModifier:1"),
            new Miscellaneous("name:Shiny rock value:8"),
            new Potion("name:Healing Draught value:10 effect:healing 10"),
            new Weapon("name:Dagger equipped:false value:10 slotsNeeded:1 minRange:1 maxRange:1 damageModifier:2 attackModifier:3"),
            new Ammunition("amount:5 value:12 name:Arrow"),
            new Armour("name:Cloth Armour equipped:false value:25 armourModifier:1 defenceModifier:1"),
            new Ring("name:Silver Ring value:30 equipped:false" ),
            new Gold(50),
            new Weapon("name:Shortsword equipped:false value:60 slotsNeeded:1 minRange:1 maxRange:2 damageModifier:4 attackModifier:4"),
            new Weapon("name:Longsword equipped:false value:80 slotsNeeded:1 minRange:1 maxRange:2 damageModifier:5 attackModifier:4"),
            new Weapon("name:Shortbow equipped:false value:80 slotsNeeded:2 minRange:2 maxRange:10 damageModifier:3 attackModifier:3"),
            new Armour("name:Leather Armour equipped:false value:85 armourModifier:2 defenceModifier:1"),
            new Armour("name:Chain Mail equipped:false value:100 armourModifier:3 defenceModifier:1"),
            new Weapon("name:Mace equipped:false value:100 slotsNeeded:1 minRange:1 maxRange:2 damageModifier:4 attackModifier:8"),
            new Ring("name:Gold Ring value:120 equipped:false"),
            new Armour("name:Duelists Doublet equipped:false value:130 armourModifier:1 defenceModifier:4"),
            new Ring("name:Jeweled Ring valuie:150 equipped:false"),
            new Weapon("name:Rapier equipped:false value:150 slotsNeeded:1 minRange:2 maxRange:3 damageModifier:5 attackModifier:2"),
            new Weapon("name:Longbow equipped:false value:150 slotsNeeded:2 minRange:3 maxRange:20 damageModifier:5 attackModifier:5"),
            new Armour("name:Plate Armour equipped:false value:180 armourModifier:8 defenceModifier:-2"),
            new Weapon("name:Greatsword equipped:false value:200 slotsNeeded:2 minRange:2 maxRange:4 damageModifier:6 attackModifier:8"),
            new Gold(200),
            new Miscellaneous("name:Wand of Nigel Thornberry value:220 effect:Smashing"),
            new Potion("name:Potion of Flight value:220 effect:Flight"),
            new Weapon("name:Greataxe equipped:false value:250 slotsNeeded:2 minRange:2 maxRange:3 damageModifier:8 attackModifier:6"),
        };
    }
}
 
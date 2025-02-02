﻿using System;
using System.Collections.Generic;
using static RPGGame.CommandManager;

namespace RPGGame
{
    internal static class GlobalConstants
    {
        //These are globally useful, but never altered during runtime

        //These are the types of items available to be made, as strings
        public static List<string> Types => types;                              
        private static readonly List<string> types = new List<string>()
            {
                "WEAPON",
                "POTION",
                "GOLD",
                "AMMUNITION",
                "ARMOUR",
                "RING",
                "MISCELLANEOUS",
        };


        //This Regex command is used to scrape attribute data from saved strings
        public static string AttFinder => attFinder;
        private const string attFinder = "(\\S+:[\\w\\s]+)(?=\\s|$)";

        //These map Regex strings to commands, to be executed anonymously during the gameloop.
        public static Dictionary<string, Action> Commands => commands;
        private static readonly Dictionary<string, Action> commands = new Dictionary<string, Action>()
                {
            { "^EXIT$", () => Exit() },
                    { "^$", () => Empty() },
                    { "^BUY\\b", () => Buy() },
                    { "^SELL\\b", () => Sell() },
                    { "^GIVE ME GOOD GRADES$", () => GrantSuper() },
                    { "^LOOK\\b|^TALK\\b", () => Look() },
                    { "\\bME\\b", () => LookAtMe() },
                    { "^INTERACT\\b", () => Interact() },
                    { "^EXAMINE\\b", () => Examine() },
                    { "^RENAME\\b", () => Rename() },
                    { "^EQUIP\\b", () => EquipByInput() },
                    { "^UNEQUIP\\b", () => UnequipByInput() },
                    { "^TAKE\\b", () => Take() },
                    { "^MUTE$", () => MuteToggle() },
                    { "^GO NORTH$|^[nN]$" , () => Move(NORTH) },
                    { "^GO SOUTH$|^[sS]$" , () => Move(SOUTH) },
                    { "^GO EAST$|^[eE]$" , () => Move(EAST) },
                    { "^GO WEST$|^[wW]$" , () => Move(WEST) },
                    { "^ADD\\b", () => Add() },
                    { "^REMOVE\\b", () => Remove() },
                    { "^SAVE$", () => Save() },
                    { "^HELP$", () => Help() },
                    { "^QUIT$", () => Quit() },
                    { "^DEMO$", () => Demo() },
                    { "", () => InvalidCommand() },
        };

        //These constant characters are used in gameboard rendering
        #region Gameboard characters and values
        public const int padding = 15;
        public const int viewDistanceWidth = 5;
        public const int viewDistanceHeight = 5;

        public const char LeftTopCornerBorder = (char)9556;
        public const char HorizontalBorder = (char)9552;
        public const char RightTopCornerBorder = (char)9559;
        public const char VerticalBorder = (char)9553;
        public const char LeftBottomCornerBorder = (char)9562;
        public const char RightBottomCornerBorder = (char)9565;

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
        #endregion

        //These are used in some text processing to apply an underline effect
        public const string UNDERLINE = "\x1B[4m";
        public const string RESET = "\x1B[0m";

        //This weapon is the default for someone at range 1 who has no other weapons available
        public static Weapon Fist = new Weapon("attackModifier:2 damageModifier:1 maxRange:1 minRange:1 slotsNeeded:1 value:0 name:Fist equipped:true");

        //These seemed more useful as readonly ints than Enums, since enums have to be fully qualified in C#???
        public static readonly int MaxHealth = 0;
        public static readonly int CurrHealth = 1;
        public static readonly int BaseArmour = 2;
        public static readonly int Speed = 3;
        public static readonly int Distance = 4;

        //These are used to make the movement control more readable within the code       
        public static readonly MoveCommand NORTH = new MoveCommand(0, -1);
        public static readonly MoveCommand SOUTH = new MoveCommand(0, 1);
        public static readonly MoveCommand EAST = new MoveCommand(1, 0);
        public static readonly MoveCommand WEST = new MoveCommand(-1, 0);

        //This list controls the behaviour of the Demo() command inside the internal test environment. They are run through and executed in order.
        public static List<string> TestCommandList => testCommandList;
        private static readonly List<string> testCommandList = new List<string>
            {
            "HELP",
            "LOOK Player",
            "ME",
            "MUTE",
            "GO NORTH",
            "GO WEST",
            "GO SOUTH",
            "GO EAST",
            "",
            "",
            "",
            "MUTE", 
            "LOOK Merchant",
            "INTERACT Merchant",
            "EXAMINE Dagger",
            "BUY Dagger",
            "EXAMINE Really Expensive Thing",
            "BUY Really Expensive Thing",
            "INTERACT Merchant",
            "ME",
            "EXAMINE Dagger",
            "EXAMINE Blade",
            "RENAME Dagger Blade",
            "ME",
            "EXAMINE Dagger",
            "EXAMINE Blade",
            "RENAME Blade Dagger",
            "EQUIP Dagger",
            "UNEQUIP Bow",
            "EQUIP Dagger",
            "ME",
            "UNEQUIP Dagger",
            "EQUIP Bow",
            "INTERACT Merchant",
            "SELL Dagger",
            "INTERACT Merchant",
            "ME",
            "ADD WEAPON name:Rapier equipped:false value:150 slotsNeeded:1 minRange:2 maxRange:3 damageModifier:5 attackModifier:2",
            "GIVE ME GOOD GRADES",
            "ADD WEAPON name:Rapier equipped:false value:150 slotsNeeded:1 minRange:2 maxRange:3 damageModifier:5 attackModifier:2",
            "ME",
            "EXAMINE Rapier",
            "REMOVE Rapier",
            "ME",
            "W",
            "",
            "",
            "",
            "LOOK Statue",
            "W",
            "INTERACT Goblin",
            "TAKE Gold",
            "TAKE Shiny Rock",
            "GIVE ME GOOD GRADES",
            "SAVE"
            };

        //This is a random item list, used for generating random inventories
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

using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class Human : Entity
    {
        private static Dictionary<String, Item[]> equiptory = new Dictionary<String, Item[]>()
        {
            { "Weapon",new Item[2]},
            { "Ring", new Item[5]},
            { "Armour", new Item[1] },
            { "Miscellaneous", new Item[5] }
        };

        public override Dictionary<string, Item[]> Equiptory { get => equiptory; set => equiptory = value; }

        public Human(String name, Coordinate position, char icon, int drawPriority, Inventory inventory, int[] stats):base(name,position,icon,drawPriority,inventory, stats){ Passive = false; }

        public Human(String name, Coordinate position, char icon, int drawPriority, int[] stats) :base(name,position,icon,drawPriority, stats) { Passive = false; }

        public Human(String name, Coordinate position, char icon, Inventory inventory, int[] stats) :base(name,position,icon,inventory, stats) { Passive = false; }

        public Human(String name, Coordinate position, char icon, int[] stats) :base(name,position,icon,stats){ Passive = false; }
    }
}
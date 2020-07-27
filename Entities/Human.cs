using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class Human : Entity
    {
        public new Dictionary<String, Item[]> equiptory = new Dictionary<String, Item[]>()
        {
            { "Weapon",new Item[2]},
            { "Ring", new Item[5]},
            { "Armour", new Item[1] },
            { "Miscellaneous", new Item[5] }
        };

        public Human(String name, Coordinate position, char icon, int drawPriority, Inventory inventory):base(name,position,icon,drawPriority,inventory){ Passive = false; }

        public Human(String name, Coordinate position, char icon, int drawPriority):base(name,position,icon,drawPriority){ Passive = false; }

        public Human(String name, Coordinate position, char icon, Inventory inventory):base(name,position,icon,inventory){ Passive = false; }

        public Human(String name, Coordinate position, char icon):base(name,position,icon){ Passive = false; }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame.Entities
{
    class Imp : Enemy
    {
        private Dictionary<String, Item[]> equiptory = new Dictionary<String, Item[]>()
        {
            { "Weapon",new Item[1]},
            { "Ring", new Item[10]},
            { "Armour", new Item[0] },
            { "Miscellaneous", new Item[5] }
        };
        public override Dictionary<string, Item[]> Equiptory { get => equiptory; set => equiptory = value; }

        public Imp(String name, Coordinate position, char icon, int drawPriority, Inventory inventory, int[] stats, string description) : base(name, position, icon, drawPriority, inventory, stats, description) { Passive = false; }

        public Imp(String name, Coordinate position, char icon, int drawPriority, int[] stats, string description) : base(name, position, icon, drawPriority, stats, description) { Passive = false; }

        public Imp(String name, Coordinate position, char icon, Inventory inventory, int[] stats, string description) : base(name, position, icon, inventory, stats, description) { Passive = false; }

        public Imp(String name, Coordinate position, char icon, int[] stats, string description) : base(name, position, icon, stats, description) { Passive = false; }
    }
}

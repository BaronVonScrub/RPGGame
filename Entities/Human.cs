using System.Collections.Generic;

namespace RPGGame
{
    internal class Human : Entity
    {
        private Dictionary<string, Item[]> equiptory = new Dictionary<string, Item[]>()
        {
            { "Weapon",new Item[2]},
            { "Ring", new Item[5]},
            { "Armour", new Item[1] },
            { "Miscellaneous", new Item[5] }
        };

        public override Dictionary<string, Item[]> Equiptory { get => equiptory; set => equiptory = value; }

        public Human(string name, Coordinate position, char icon, int drawPriority, Inventory inventory, int[] stats, string description) : base(name, position, icon, drawPriority, inventory, stats, description) => Passive = false;

        /*public Human(String name, Coordinate position, char icon, int drawPriority, int[] stats, string description) :base(name,position,icon,drawPriority, stats,description) { Passive = false; }*/

        public Human(string name, Coordinate position, char icon, Inventory inventory, int[] stats, string description) : base(name, position, icon, inventory, stats, description) => Passive = false;

        /*public Human(String name, Coordinate position, char icon, int[] stats, string description) :base(name,position,icon,stats, description) { Passive = false; }*/
    }
}
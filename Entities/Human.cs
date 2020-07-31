using System.Collections.Generic;

namespace RPGGame
{
    internal class Human : Entity
    {
        //Override passive status to be false;
        public override bool Passive { get; internal set; } = false;

        //This dictionary both stores and limits the use of items of different types.
        public override Dictionary<string, Item[]> Equiptory { get => equiptory; set => equiptory = value; }
        private Dictionary<string, Item[]> equiptory = new Dictionary<string, Item[]>()
        {
            { "Weapon",new Item[2]},
            { "Ring", new Item[5]},
            { "Armour", new Item[1] },
            { "Miscellaneous", new Item[5] }
        };

        //Basic inherited constructor
        public Human(string name, Coordinate position, char icon, int drawPriority, Inventory inventory, int[] stats, string description) : base(name, position, icon, drawPriority, inventory, stats, description) { }
    }
}
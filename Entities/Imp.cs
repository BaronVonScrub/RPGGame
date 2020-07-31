using System.Collections.Generic;

namespace RPGGame.Entities
{
    internal class Imp : Enemy
    {
        //Overrides the basic Equiptory to one specific to the imp class
        public override Dictionary<string, Item[]> Equiptory { get => equiptory; set => equiptory = value; }
        private Dictionary<string, Item[]> equiptory = new Dictionary<string, Item[]>()
        {
            { "Weapon",new Item[1]},
            { "Ring", new Item[10]},
            { "Armour", new Item[0] },
            { "Miscellaneous", new Item[5] }
        };

        //Basic inherited constructor
        public Imp(string name, Coordinate position, char icon, int drawPriority, Inventory inventory, int[] stats, string description) : base(name, position, icon, drawPriority, inventory, stats, description) => Passive = false;
    }
}

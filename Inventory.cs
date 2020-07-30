using System.Collections.Generic;

namespace RPGGame
{
    internal class Inventory
    {
        public string name;
        public List<Item> inventData;

        public Inventory(string name, List<Item> inventData)
        {
            this.name = name;
            this.inventData = inventData;
        }

        public Inventory(string name)
        {
            this.name = name;
            inventData = new List<Item>();
        }

        public Item GetItem(string itemName)
        {
            Item item = inventData.Find(x => x.Name == itemName);
            return item;
        }
    }
}

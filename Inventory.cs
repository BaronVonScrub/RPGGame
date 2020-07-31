using System.Collections.Generic;

namespace RPGGame
{
    internal class Inventory
    {
        public string name;                             //Stores a name for the inventory
        public List<Item> inventData;                   //Stores the actual items

        //Basic constructor
        public Inventory(string name, List<Item> inventData)
        {
            this.name = name;
            this.inventData = inventData;
        }
        
        //Constructor if no item list is provided
        public Inventory(string name)
        {
            this.name = name;
            inventData = new List<Item>();
        }

        //Gets an item from the inventory by name
        public Item GetItem(string itemName)
        {
            Item item = inventData.Find(x => x.Name == itemName);
            return item;
        }
    }
}

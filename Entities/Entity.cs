using System;
using System.Collections.Generic;
using static RPGGame.GlobalVariables;
using static RPGGame.ConstantVariables;

namespace RPGGame
{
    class Entity
    {
        public Coordinate position = new Coordinate();
        public Inventory inventory;
        public List<Item> equipped = new List<Item>();
        public char icon = (char)32;
        public int drawPriority = 0;
        private string name;
        private Boolean passive;
        private Boolean impassable;
        protected Dictionary<String, Item[]> equiptory = new Dictionary<string, Item[]>();

        public string Name { get => name; set => name = value; }
        public bool Passive { get => passive; set => passive = value; }
        public bool Impassable { get => impassable; set => impassable = value; }

        #region Constructors
        public Entity() { }

        public Entity(String name, Coordinate position, char icon, int drawPriority, Inventory inventory)
        {
            this.Name = name;
            this.position = position;
            this.inventory = inventory;
            this.icon = icon;
            this.drawPriority = drawPriority;
            Passive = true;
            Impassable = false;
            if (inventory != null)
                if (!Inventories.Contains(inventory))
                    Inventories.Add(inventory);
            EquipUpdate();
        }

        public Entity(String name, Coordinate position, char icon, int drawPriority)
        {
            this.Name = name;
            this.position = position;
            this.icon = icon;
            this.drawPriority = drawPriority;
            this.inventory = new Inventory(name);
            Passive = true;
            Impassable = false;
            if (inventory != null)
                if (!Inventories.Contains(inventory))
                    Inventories.Add(inventory);
            EquipUpdate();
        }

        public Entity(String name, Coordinate position, char icon, Inventory inventory)
        {
            this.Name = name;
            this.position = position;
            this.inventory = inventory;
            this.drawPriority = 1;
            this.icon = icon;
            Passive = true;
            Impassable = false;
            if (inventory != null)
                if (!Inventories.Contains(inventory))
                    Inventories.Add(inventory);
            EquipUpdate();
        }

        public Entity(String name, Coordinate position, char icon)
        {
            this.Name = name;
            this.position = position;
            this.icon = icon;
            this.drawPriority = 0;
            this.inventory = new Inventory(name);
            Passive = true;
            Impassable = false;
            if (inventory != null)
                if (!Inventories.Contains(inventory))
                    Inventories.Add(inventory);
            EquipUpdate();
        }
        #endregion

        public View GetView()
        {
            return new View(
                new Coordinate(
                    position.x - viewDistanceWidth,
                    position.y - viewDistanceHeight
                    ),
                new Coordinate(
                    position.x + viewDistanceWidth,
                    position.y + viewDistanceHeight
                    )
                );
        }

        public void EquipUpdate()
        {
            if (inventory == null)
                return;
            foreach (Item item in inventory.inventData.FindAll(x => x.Equipped = true))
            {
                Boolean done = false;
                item.Equipped = true;

                if (equiptory.ContainsKey(item.GetType().Name))
                    for (int i = 0; i < equiptory[item.GetType().Name].Length; i++)
                        if (equiptory[item.GetType().Name][i] == null)
                        {
                            equiptory[item.GetType().Name][i] = item;
                            done = true;
                            break;
                        }
                if (!done)
                    item.Equipped = false;
            }
        }

        public Boolean Equip(Item item)
        {
            Boolean done = false;
            if (item.itemData["equipped"] == "true")
            {
                item.Equipped = true;
                for (int i = 0; i < equiptory[item.GetType().Name].Length; i++)
                    if (equiptory[item.GetType().Name][i] == null)
                    {
                        equiptory[item.GetType().Name][i] = item;
                        done = true;
                        break;
                    }
            }
            if (!done)
            {
                item.Equipped = false;
                return false;
            }
            return true;
        }
    }
}
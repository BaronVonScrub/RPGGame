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
        private Boolean passable;
        protected Dictionary<String, Item[]> equiptory = new Dictionary<string, Item[]>();
        public int[] stats = new int[] {0, 0, 0, 0, 0, 0};

        public string Name { get => name; set => name = value; }
        public bool Passive { get => passive; set => passive = value; }
        public bool Passable { get => passable; set => passable = value; }

        #region Constructors
        public Entity() { }

        public Entity(String name, Coordinate position, char icon, int drawPriority, Inventory inventory, int[] stats)
        {
            this.Name = name;
            this.position = position;
            this.inventory = inventory;
            this.icon = icon;
            this.drawPriority = drawPriority;
            this.stats = stats;
            Passive = true;
            Passable = true;
            if (inventory != null)
                if (!Inventories.Contains(inventory))
                    Inventories.Add(inventory);
            EquipUpdate();
        }

        public Entity(String name, Coordinate position, char icon, int drawPriority, int[] stats)
        {
            this.Name = name;
            this.position = position;
            this.icon = icon;
            this.drawPriority = drawPriority;
            this.inventory = new Inventory(name);
            this.stats = stats;
            Passive = true;
            Passable = true;
            if (inventory != null)
                if (!Inventories.Contains(inventory))
                    Inventories.Add(inventory);
            EquipUpdate();
        }

        public Entity(String name, Coordinate position, char icon, Inventory inventory, int[] stats)
        {
            this.Name = name;
            this.position = position;
            this.inventory = inventory;
            this.drawPriority = 1;
            this.icon = icon;
            this.stats = stats;
            Passive = true;
            Passable = true;
            if (inventory != null)
                if (!Inventories.Contains(inventory))
                    Inventories.Add(inventory);
            EquipUpdate();
        }

        public Entity(String name, Coordinate position, char icon, int[] stats)
        {
            this.Name = name;
            this.position = position;
            this.icon = icon;
            this.drawPriority = 0;
            this.inventory = new Inventory(name);
            this.stats = stats;
            Passive = true;
            Passable = true;
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
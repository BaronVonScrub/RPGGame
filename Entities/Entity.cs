using System;
using System.Collections.Generic;
using static RPGGame.GlobalVariables;
using static RPGGame.ConstantVariables;
using static RPGGame.TextTool;

namespace RPGGame
{
    class Entity
    {
        public Coordinate position = new Coordinate();
        public Inventory inventory;
        public char icon = (char)32;
        public int drawPriority = 0;
        private string name;
        private Boolean passive;
        private Boolean passable;
        private Dictionary<String, Item[]> equiptory = new Dictionary<string, Item[]>();
        protected int[] stats = new int[] { 0, 0, 0, 0, 0, 0 };

        public string Name { get => name; set => name = value; }
        public bool Passive { get => passive; set => passive = value; }
        public bool Passable { get => passable; set => passable = value; }

        public Boolean Unequip(Item item)
        {
            if (item.Equipped == false)
            {
                WriteLine("That item is not equipped!");
                return false;
            }

            string itemType = item.GetType().Name;
            if (!this.Equiptory.ContainsKey(itemType))
            {
                WriteLine("That shouldn't be equippable; naughty!");
                item.Equipped = false;
                return false;
            }

            Boolean done = false;
            for (int i = 0; i < Equiptory[itemType].Length; i++)
                if (Equiptory[itemType][i] == item)
                {
                    item.Equipped = false;
                    Equiptory[itemType][i] = null;
                    done = true;
                    break;
                }
            if (!done)
            {
                WriteLine(itemType + "Item not found in equiptory; weird.");
                item.Equipped = false;
                return false;
            }

            WriteLine(item.Name + " unequipped!");
            return true;
        }

        public int[] Stats { get => stats; set => stats = value; }
        public virtual Dictionary<string, Item[]> Equiptory { get => equiptory; set => equiptory = value; }

        #region Constructors
        public Entity() { }

        public Entity(String name, Coordinate position, char icon, int drawPriority, Inventory inventory, int[] stats)
        {
            this.Name = name;
            this.position = position;
            this.inventory = inventory;
            this.icon = icon;
            this.drawPriority = drawPriority;
            this.Stats = stats;
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
            this.Stats = stats;
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
            this.Stats = stats;
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
            this.Stats = stats;
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
            foreach (Item item in inventory.inventData.FindAll(x => x.Equipped == true))
            {
                Boolean done = false;
                if (Equiptory.ContainsKey(item.GetType().Name))
                    for (int i = 0; i < Equiptory[item.GetType().Name].Length; i++)
                        if (Equiptory[item.GetType().Name][i] == null)
                        {
                            Equiptory[item.GetType().Name][i] = item;
                            item.Equipped = true;
                            done = true;
                            break;
                        }
                if (!done)
                    item.Equipped = false;
            }
        }

        public Boolean Equip(Item item)
        {
            if (item.Equipped == true)
            {
                WriteLine("That item is already equipped!");
                return false;
            }

            string itemType = item.GetType().Name;
            if (!this.Equiptory.ContainsKey(itemType))
            {
                WriteLine("That can't be equipped!");
                return false;
            }

            Boolean done = false;
            for (int i = 0; i < Equiptory[itemType].Length; i++)
                if (Equiptory[itemType][i] == null)
                {
                    item.Equipped = true;
                    Equiptory[itemType][i] = item;
                    done = true;
                    break;
                }
            if (!done)
            {
                WriteLine(itemType+" slots are full!");
                return false;
            }

            WriteLine(item.Name + " equipped!");
            return true;
        }

        internal void GetStats()
        {
            throw new NotImplementedException();
        }
    }
}
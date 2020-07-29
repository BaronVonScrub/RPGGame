using System;
using System.Collections.Generic;
using static RPGGame.GlobalVariables;
using static RPGGame.ConstantVariables;
using static RPGGame.TextTool;
using System.Linq;
using System.Globalization;

namespace RPGGame
{
    class Entity
    {
        public Coordinate position = new Coordinate();
        public Inventory inventory;
        public char icon = (char)32;
        public int drawPriority = 0;

        internal int GetMaxRange()
        {
            int temp = 1;
            if (!Equiptory.ContainsKey("Weapon"))
                return temp;
            
            foreach(Item weapon in Equiptory["Weapon"]) {
                if (weapon == null)
                    continue;
                int range = Int32.Parse(weapon.itemData["maxRange"]);
                if (range > temp)
                temp = range;
            }
            return temp;
        }

        internal int GetMinRange()
        {
            int temp = 1;
            if (!Equiptory.ContainsKey("Weapon"))
                return 1;

            foreach (Item weapon in Equiptory["Weapon"])
            {
                if (weapon == null)
                    continue;
                int range = Int32.Parse(weapon.itemData["minRange"]);
                if (range > temp)
                    temp = range;
            }
            return temp;
        }

        internal List<Weapon> GetEquippedWeapons()
        {
            List<Weapon> tempList = new List<Weapon>() { Fist };
            if (!Equiptory.ContainsKey("Weapon"))
                return tempList;
            foreach (Weapon currWeapon in Equiptory["Weapon"])
                if (currWeapon != null)
                    tempList.Add(currWeapon);
            return tempList;
        }

        private Dictionary<String, Item[]> equiptory = new Dictionary<string, Item[]>();
        protected int[] stats = new int[] { 0, 0, 0, 0, 0 };

        public string Name { get; set; }
        public bool Passive { get; set; }
        public bool Passable { get; set; }

        internal int GetDefence()
        {
            int def = 0;
            if (!Equiptory.ContainsKey("Armour"))
                return def;
            foreach (Armour arm in Equiptory["Armour"])
                def += arm.Get("defenceModifier");
            return def;
        }

        internal int GetSpeed()
        {
            return Stats[Speed];
        }

        internal int GetArmour()
        {
            int ar = Stats[BaseArmour];
            if (!Equiptory.ContainsKey("Armour"))
                return ar;
            foreach (Armour arm in Equiptory["Armour"])
                ar += arm.Get("armourModifier");
            return ar;
        }

        internal int DistanceFromCenter()
        {
            return (int)Math.Round(Math.Sqrt((position.x * position.x) + (position.y * position.y)));
        }

        public Boolean Unequip(Item item)
        {
            if (item.Equipped == false)
            {
                WriteLine("That item is not currently equipped!");
                return false;
            }

            string itemType = item.GetType().Name;

            if (!Equiptory.ContainsKey(itemType))
            {
                WriteLine("That shouldn't be equippable! Naughty.");
                item.Equipped = false;
                return true;
            }

            int slotsRequired = 1;
            if (itemType == "Weapon")
                slotsRequired = (item as Weapon).slotsRequired;

            if (Equiptory[itemType].ToList().FindAll(x => x == item).Count < slotsRequired)
            {
                WriteLine("That shouldn't be equippable! Naughty.");
                for (int j = 0; j < slotsRequired; j++)
                    for (int i = 0; i < Equiptory[itemType].Length; i++)
                        if (Equiptory[item.GetType().Name][i] == item)
                        {
                            Equiptory[item.GetType().Name][i] = null;
                            break;
                        }
                item.Equipped = false;
                return true;
            }

            item.Equipped = false;

            for (int j = 0; j < slotsRequired; j++)
                for (int i = 0; i < Equiptory[item.GetType().Name].Length; i++)
                    if (Equiptory[item.GetType().Name][i] == item)
                    {
                        Equiptory[item.GetType().Name][i] = null;
                        break;
                    }
            return true;
        }

        internal void Die()
        {
            Dead = true;
            Passive = true;
            Aggressive = false;
            Status = " (Dead)";
            icon = (char)9604;
        }

        public int[] Stats { get => stats; set => stats = value; }
        public virtual Dictionary<string, Item[]> Equiptory { get => equiptory; set => equiptory = value; }
        public virtual bool Aggressive { get; internal set; } = false;
        public bool Dead { get; set; } = false;
        public string Status { get; internal set; } = "";

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

        internal void StatDisplay()
        {
            WriteLine("Health : "+Stats[CurrHealth].ToString()+"/"+Stats[MaxHealth].ToString());
            WriteLine("Def : " + GetDefence().ToString() + "   Arm : " + GetArmour().ToString());
            WriteLine("Speed : " + GetSpeed().ToString());
        }

        internal void SetHealth(int inHealth)
        {
            Stats[CurrHealth] = inHealth;
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
            if (inventory.inventData.Count == 0)
                return;

            foreach (Item item in inventory.inventData.FindAll(x => x.Equipped == true))
            {
                item.Equipped = false;
                Equip(item);
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

            if (!Equiptory.ContainsKey(itemType))
            {
                WriteLine("Can't equip that!");
                return false;
            }

            int slotsRequired = 1;
            if (itemType == "Weapon")
                slotsRequired = (item as Weapon).slotsRequired;

            if (Equiptory[item.GetType().Name].ToList().FindAll(x => x == null).Count < slotsRequired)
            {
                WriteLine("Not enough slots!");
                item.Equipped = false;
                return false;
            }

            item.Equipped = true;

            for (int j = 0; j < slotsRequired; j++)
                for (int i = 0; i < Equiptory[item.GetType().Name].Length; i++)
                    if (Equiptory[item.GetType().Name][i] == null)
                    {
                        Equiptory[item.GetType().Name][i] = item;
                        break;
                    }
            return true;
        }
    }
}
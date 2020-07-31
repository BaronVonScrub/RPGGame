using System;
using System.Collections.Generic;
using System.Linq;
using static RPGGame.GlobalConstants;
using static RPGGame.EntityManager;
using static RPGGame.GlobalVariables;
using static RPGGame.TextManager;

namespace RPGGame
{
    internal class Entity
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

            foreach (Item weapon in Equiptory["Weapon"])
            {
                if (weapon == null)
                    continue;
                int range = int.Parse(weapon.itemData["maxRange"]);
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
                int range = int.Parse(weapon.itemData["minRange"]);
                if (range > temp)
                    temp = range;
            }
            return temp;
        }

        internal List<Weapon> GetEquippedWeapons()
        {
            var tempList = new List<Weapon>() { Fist };
            if (!Equiptory.ContainsKey("Weapon"))
                return tempList;
            foreach (Weapon currWeapon in Equiptory["Weapon"])
                if (currWeapon != null)
                    tempList.Add(currWeapon);
            return tempList;
        }

        private Dictionary<string, Item[]> equiptory = new Dictionary<string, Item[]>();
        protected int[] stats = new int[] { 0, 0, 0, 0, 0 };

        public string Name { get; set; }
        public bool Passive { get; set; }
        public bool Passable { get; set; }

        internal int GetDefence()
        {
            int def = 0;
            if (!Equiptory.ContainsKey("Armour"))
                return def;
            if (Equiptory["Armour"].ToList().Exists(x => x != null))
                foreach (Armour arm in Equiptory["Armour"].ToList().FindAll(x => x != null))
                    def += arm.Get("defenceModifier");
            return def;
        }

        internal int GetSpeed() => Stats[Speed];

        internal int GetArmour()
        {
            int ar = Stats[BaseArmour];
            if (!Equiptory.ContainsKey("Armour"))
                return ar;
            if (Equiptory["Armour"].ToList().Exists(x => x != null))
                foreach (Armour arm in Equiptory["Armour"].ToList().FindAll(x => x != null))
                    ar += arm.Get("armourModifier");
            return ar;
        }

        internal int GetAttack()
        {
            List<Weapon> alreadyCounted = new List<Weapon>();
            int att = 1;
            if (!Equiptory.ContainsKey("Weapon"))
                return att;
            if (Equiptory["Weapon"].ToList().Exists(x => x != null))
                foreach (Weapon weap in Equiptory["Weapon"].ToList().FindAll(x => x != null))
                {
                    if (!alreadyCounted.Contains(weap))
                    {
                        att += weap.Get("attackModifier");
                        alreadyCounted.Add(weap);
                    }
                }
            return att;
        }

        internal int GetDamage()
        {
            List<Weapon> alreadyCounted = new List<Weapon>();
            int dam = 1;
            if (!Equiptory.ContainsKey("Weapon"))
                return dam;
            if (Equiptory["Weapon"].ToList().Exists(x => x != null))
                foreach (Weapon weap in Equiptory["Weapon"].ToList().FindAll(x => x != null))
                {
                    if (!alreadyCounted.Contains(weap))
                    {
                        dam += weap.Get("damageModifier");
                        alreadyCounted.Add(weap);
                    }
                }
            return dam;
        }

        internal int DistanceFromCenter() => (int)Math.Round(Math.Sqrt((position.x * position.x) + (position.y * position.y)));

        public bool UnequipByItem(Item item)
        {
            if (!inventory.inventData.Exists(x => x == item))
            {
                WriteLine("Item not found!");
                return false;
            }
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
        public string Description { get; internal set; } = "NO DESCRIPTION SET";

        #region Constructors
        public Entity() { }

        public Entity(string name, Coordinate position, char icon, int drawPriority, Inventory inventory, int[] stats, string description)
        {
            Name = name;
            this.position = position;
            this.inventory = inventory;
            this.icon = icon;
            this.drawPriority = drawPriority;
            Stats = stats;
            Description = description;
            Passive = true;
            Passable = true;
            if (inventory != null)
                if (!Inventories.Contains(inventory))
                    Inventories.Add(inventory);
            Description = description;
            EquipUpdate();
        }

        internal void StatDisplay()
        {
            WriteLine("Health : " + Stats[CurrHealth].ToString() + "/" + Stats[MaxHealth].ToString());
            WriteLine("Def : " + GetDefence().ToString() + "   Arm : " + GetArmour().ToString());
            WriteLine("Att : " + GetAttack().ToString() + "   Dam : " + GetDamage().ToString());
            WriteLine("Speed : " + GetSpeed().ToString());
        }

        internal void SetHealth(int inHealth) => Stats[CurrHealth] = inHealth;

        /*public Entity(String name, Coordinate position, char icon, int drawPriority, int[] stats, string description)
                {
                    this.Name = name;
                    this.position = position;
                    this.icon = icon;
                    this.drawPriority = drawPriority;
                    this.inventory = new Inventory(name);
                    this.Stats = stats;
                    this.Description = description;
                    Passive = true;
                    Passable = true;
                    if (inventory != null)
                        if (!Inventories.Contains(inventory))
                            Inventories.Add(inventory);
                    EquipUpdate();
                }*/

        public Entity(string name, Coordinate position, char icon, Inventory inventory, int[] stats, string description)
        {
            Name = name;
            this.position = position;
            this.inventory = inventory;
            drawPriority = 1;
            this.icon = icon;
            Stats = stats;
            Description = description;
            Passive = true;
            Passable = true;
            if (inventory != null)
                if (!Inventories.Contains(inventory))
                    Inventories.Add(inventory);
            EquipUpdate();
        }

        /*public Entity(String name, Coordinate position, char icon, int[] stats, string description)
        {
            this.Name = name;
            this.position = position;
            this.icon = icon;
            this.drawPriority = 0;
            this.inventory = new Inventory(name);
            this.Stats = stats;
            this.Description = description;
            Passive = true;
            Passable = true;
            if (inventory != null)
                if (!Inventories.Contains(inventory))
                    Inventories.Add(inventory);
            EquipUpdate();
        }*/
        #endregion

        public View GetView() => new View(
                new Coordinate(
                    position.x - viewDistanceWidth,
                    position.y - viewDistanceHeight
                    ),
                new Coordinate(
                    position.x + viewDistanceWidth,
                    position.y + viewDistanceHeight
                    )
                );

        public void EquipUpdate()
        {
            if (inventory == null)
                return;
            if (inventory.inventData.Count == 0)
                return;

            foreach (Item item in inventory.inventData.FindAll(x => x.Equipped == true))
            {
                item.Equipped = false;
                EquipToTargetByItem(this, item);
            }
        }
    }
}
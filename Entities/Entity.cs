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
        #region Properties
        public Coordinate position = new Coordinate();                                                                  //Coordinate on the gameboard
        protected int[] stats = new int[] { 0, 0, 0, 0, 0 };                                                            //The stats

        public Inventory Inventory { get; set; }                                                                        //The inventory
        public char Icon { get; set; } = (char)32;                                                                      //The representative character
        public int DrawPriority { get; set; } = 0;                                                                      //The drawing priority when multiple entities are in one place
        public int[] Stats { get; set; }                                                                                //The stats
        public virtual Dictionary<string, Item[]> Equiptory { get; set; }                                               //Matches a string (storing Item type) to an array of items, storing and limiting the slots 
        public string Description { get; internal set; } = "NO DESCRIPTION SET";                                        //Description for ingame use
        public string Name { get; set; }                                                                                //Name for easy reference in game and in code
        public bool Dead { get; internal set; } = false;                                                                //Bool recording if the entity is dead
        public virtual bool Passive { get; internal set; }                                                              //Can you freely take things from the inventory of the entity?
        public virtual bool Passable { get; internal set; }                                                             //Can you enter a square with the entity?
        public virtual bool Aggressive { get; internal set; } = false;                                                  //Do you enter combat if you move into a square with this entity
        public string Status { get; set; } = "";                                                                        //String storing status effect, appended to name in "LOOK" command
#endregion

        #region Constructors
        public Entity() { }

        //Please note that while inherited constructors may have no references in code, they ARE used as they are called by reflection.
        //Full constructor
        public Entity(string name, Coordinate position, char icon, int drawPriority, Inventory inventory, int[] stats, string description)
        {
            Name = name;
            this.position = position;
            this.Inventory = inventory;
            this.Icon = icon;
            this.DrawPriority = drawPriority;
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

        //Shorthand constructor for use by the wall class
        public Entity(string name, Coordinate position, char icon, Inventory inventory, int[] stats, string description)
        {
            Name = name;
            this.position = position;
            this.Inventory = inventory;
            DrawPriority = 1;
            this.Icon = icon;
            Stats = stats;
            Description = description;
            Passive = true;
            Passable = true;
            if (inventory != null)
                if (!Inventories.Contains(inventory))
                    Inventories.Add(inventory);
            EquipUpdate();
        }
        #endregion

        #region Map and interaction
        //This displays the stats of the entity, for use in inventory viewing.
        internal void StatDisplay()
        {
            WriteLine("Health : " + Stats[CurrHealth].ToString() + "/" + Stats[MaxHealth].ToString());
            WriteLine("Def : " + GetDefence().ToString() + "   Arm : " + GetArmour().ToString());
            WriteLine("Att : " + GetAttack().ToString() + "   Dam : " + GetDamage().ToString());
            WriteLine("Speed : " + GetSpeed().ToString());
        }

        //Just pythagoras, returns the entity's position from the center. Used for scaling monster frequency (And possibly level in the future?)
        internal int DistanceFromCenter() => (int)Math.Round(Math.Sqrt((position.x * position.x) + (position.y * position.y)));


        //Gets the view from the entity's POV
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
        #endregion

        #region Combat
        //Gets the maximum range of all the items equipped by the entity. This determines the starting range of combat.
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

        //Gets the minimum range of all items equipped by the entity. If this is larger than 1, an entity defaults to fists at range 1
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

        //Gets a list of all equipped weapons, defaulting to fists if there are none.
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
        
        //Returns the total defence stat of the entity based on armour
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

        //Returns the speed stat of the entity
        internal int GetSpeed() => Stats[Speed];

        //Returns the total armour stat of the entity based on base armour + armour
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

        //Returns the attack of all weapons (Misleading as you cannot always use them all in combat)
        internal int GetAttack()
        {
            List<Weapon> alreadyCounted = new List<Weapon>();                                           //This list stores items as they are read, to stop multi-slot-filling items from being included more than once
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

        //Returns the damager of all weapons (Misleading as you cannot always use them all in combat)
        internal int GetDamage()
        {
            List<Weapon> alreadyCounted = new List<Weapon>();                                           //This list stores items as they are read, to stop multi-slot-filling items from being included more than once
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

        //This sets the health inside of the stats array
        internal void SetHealth(int inHealth) => Stats[CurrHealth] = inHealth;

        //Death routine for entities.
        internal void Die()
        {
            Dead = true;
            Passive = true;
            Aggressive = false;
            Status = " (Dead)";
            Icon = (char)9604;
        }
        #endregion

        #region Equipment
        //Unequip an item by the item itself
        public bool UnequipByItem(Item item)
        {
            #region Escape conditions
            if (!Inventory.inventData.Exists(x => x == item))
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
            #endregion

            int slotsRequired = 1;
            if (itemType == "Weapon")
                slotsRequired = (item as Weapon).slotsRequired;                                 //Note that only weapons can currently require multiple slots


            if (Equiptory[itemType].ToList().FindAll(x => x == item).Count < slotsRequired)     //If the item is somehow equipped despite lacking the slots for it
            {
                WriteLine("That shouldn't be equippable! Naughty.");                            //Remove the item from the slots and mark it as unequipped.
                for (int j = 0; j < slotsRequired; j++)
                    for (int i = 0; i < Equiptory[itemType].Length; i++)
                        if (Equiptory[item.GetType().Name][i] == item)
                        {
                            Equiptory[item.GetType().Name][i] = null;
                            break;                                                              //
                        }
                item.Equipped = false;                                                          
                return true;                                                                    //Returns the unequip as successful
            }

            item.Equipped = false;                                                              //Mark the item as unequipped

            for (int j = 0; j < slotsRequired; j++)                                             //Remove the item from the slots
                for (int i = 0; i < Equiptory[item.GetType().Name].Length; i++)
                    if (Equiptory[item.GetType().Name][i] == item)
                    {
                        Equiptory[item.GetType().Name][i] = null;
                        break;                                                                  //
                    }
            return true;                                                                        //Return the unequip as successful
        }

        //Updates the equip status and slots of all items
        public void EquipUpdate()
        {
            if (Inventory == null)
                return;
            if (Inventory.inventData.Count == 0)
                return;

            foreach (Item item in Inventory.inventData.FindAll(x => x.Equipped == true))
            {
                item.Equipped = false;
                EquipToTargetByItem(this, item);
            }
        }
        #endregion
    }
}
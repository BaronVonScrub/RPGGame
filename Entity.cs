﻿using System;
using System.Collections.Generic;
using static RPGGame.GlobalVariables;

namespace RPGGame
{
    class Entity
    {
        public Coordinate position = new Coordinate();
        public Inventory inventory;
        public List<Item> equipped = new List<Item>();
        public char icon = (char)32;
        public int drawPriority = 0;
        public string name;
        protected Dictionary<String, Item[]> equiptory = new Dictionary<string, Item[]>();

        #region Constructors
        public Entity() { }

        public Entity(String name, Coordinate position, char icon, int drawPriority, Inventory inventory)
        {
            this.name = name;
            this.position = position;
            this.inventory = inventory;
            this.icon = icon;
            this.drawPriority = drawPriority;
            if (inventory != null)
                if (!Inventories.Contains(inventory))
                    Inventories.Add(inventory);
        }

        public Entity(String name, Coordinate position, char icon, int drawPriority)
        {
            this.name = name;
            this.position = position;
            this.icon = icon;
            this.drawPriority = drawPriority;
            this.inventory = new Inventory(name);
            if (inventory != null)
                if (!Inventories.Contains(inventory))
                    Inventories.Add(inventory);
        }

        public Entity(String name, Coordinate position, char icon, Inventory inventory)
        {
            this.name = name;
            this.position = position;
            this.inventory = inventory;
            this.drawPriority = 0;
            this.icon = icon;
            if (inventory != null)
                if (!Inventories.Contains(inventory))
                    Inventories.Add(inventory);
        }

        public Entity(String name, Coordinate position, char icon)
        {
            this.name = name;
            this.position = position;
            this.icon = icon;
            this.drawPriority = 0;
            this.inventory = new Inventory(name);
            if (inventory != null)
                if (!Inventories.Contains(inventory))
                    Inventories.Add(inventory);
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
            foreach (Item item in inventory.inventData)
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

    struct View
    {
        public Coordinate topLeft;
        public Coordinate bottomRight;

        public View(Coordinate topLeft, Coordinate bottomRight)
        {
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
        }
    }
}
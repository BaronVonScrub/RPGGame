using System;
using System.Collections.Generic;
using System.Text;

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

        #region Constructors
        public Entity(String name, Coordinate position, char icon, int drawPriority, Inventory inventory)
        {
            this.name = name;
            this.position = position;
            this.inventory = inventory;
            this.icon = icon;
            this.drawPriority = drawPriority;
            if (inventory != null)
                if (!InventoryManager.inventories.Contains(inventory))
                    InventoryManager.inventories.Add(inventory);
        }

        public Entity(String name, Coordinate position, char icon, int drawPriority)
        {
            this.name = name;
            this.position = position;
            this.icon = icon;
            this.drawPriority = drawPriority;
            this.inventory = new Inventory(name);
            if (inventory != null)
                if (!InventoryManager.inventories.Contains(inventory))
                    InventoryManager.inventories.Add(inventory);
        }

        public Entity(String name, Coordinate position, char icon, Inventory inventory)
        {
            this.name = name;
            this.position = position;
            this.inventory = inventory;
            this.drawPriority = 0;
            this.icon = icon;
            if (inventory!=null)
                if (!InventoryManager.inventories.Contains(inventory))
                    InventoryManager.inventories.Add(inventory);
        }

        public Entity(String name, Coordinate position, char icon)
        {
            this.name = name;
            this.position = position;
            this.icon = icon;
            this.drawPriority = 0;
            this.inventory = new Inventory(name);
            if (inventory != null)
                if (!InventoryManager.inventories.Contains(inventory))
                    InventoryManager.inventories.Add(inventory);
        }
        #endregion

        public View GetView()
        {
            return new View(
                new Coordinate(
                    position.x - GameBoard.viewDistanceWidth,
                    position.y - GameBoard.viewDistanceHeight
                    ),
                new Coordinate(
                    position.x + GameBoard.viewDistanceWidth,
                    position.y + GameBoard.viewDistanceHeight
                    )
                );
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
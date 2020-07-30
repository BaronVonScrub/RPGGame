using System;

namespace RPGGame
{
    internal struct Coordinate
    {
        public int x;
        public int y;

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Coordinate(string[] inp)
        {
            x = int.Parse(inp[0]);
            y = int.Parse(inp[1]);
        }
    }

    internal struct View
    {
        public Coordinate topLeft;
        public Coordinate bottomRight;

        public View(Coordinate topLeft, Coordinate bottomRight)
        {
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
        }
    }

    internal struct Line
    {
        public string lineData;
        public ConsoleColor col;

        public Line(string lineData, ConsoleColor col)
        {
            this.lineData = lineData;
            this.col = col;
        }
    }

    internal struct EntityData
    {
        public string type;
        public string name;
        public char icon;
        public int drawPriority;
        public Coordinate position;
        public Inventory inventory;
        public int[] stats;
        public string description;

        public EntityData(string[] data)
        {
            type = data[1];
            name = data[0];
            icon = (char)int.Parse(data[2]);
            drawPriority = int.Parse(data[3]);
            position = new Coordinate(data[4].Split(" "));
            inventory = null;

            if (data[5] != "Null")
                inventory = InventoryManager.GetInventory(data[5]);

            string[] tempStats = data[6].Split(" ");
            stats = new int[tempStats.Length];
            for (int i = 0; i < stats.Length; i++)
                stats[i] = int.Parse(tempStats[i]);

            description = data[7];
        }
    }

    internal struct ItemData
    {
        public string type;
        public string data;
        public ItemData(string inData)
        {
            type = ParseManager.FirstUpperOnly(ParseManager.GetItemType(inData));
            data = ParseManager.Strip(inData);
        }
    }
}

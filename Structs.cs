using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    struct Coordinate
    {
        public int x;
        public int y;

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Coordinate(String[] inp)
        {
            this.x = Int32.Parse(inp[0]);
            this.y = Int32.Parse(inp[1]);
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
    struct Line
    {
        public string lineData;
        public ConsoleColor col;

        public Line(String lineData, ConsoleColor col)
        {
            this.lineData = lineData;
            this.col = col;
        }
    }

    struct EntityData
    {
        public string type;
        public string name;
        public char icon;
        public int drawPriority;
        public Coordinate position;
        public Inventory inventory;
        public int[] stats;

        public EntityData(string[] data)
        {
            type = data[1];
            name = data[0];
            icon = (char)Int32.Parse(data[2]);
            drawPriority = Int32.Parse(data[3]);
            position = new Coordinate(data[4].Split(" "));
            inventory = null;

            if (data[5] != "Null")
                inventory = InventoryManager.GetInventory(data[5]);

            string[] tempStats = data[6].Split(" ");
            stats = new int[tempStats.Length];
            for (int i = 0; i < stats.Length; i++)
                stats[i] = Int32.Parse(tempStats[i]);
        }
    }

    struct ItemData
    {
        public string type;
        public string data;
        public ItemData(string inData) {
            type = ParseTool.FirstUpperOnly(ParseTool.GetItemType(inData));
            data = ParseTool.Strip(inData);
        }
    }
}

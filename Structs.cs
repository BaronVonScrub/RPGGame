using System;

namespace RPGGame
{
    //Coordinate for use on the map
    internal struct Coordinate
    {
        public int x;
        public int y;

        //Constructor with values
        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        //Constructor with string in form "X Y"
        public Coordinate(string[] inp)
        {
            x = int.Parse(inp[0]);
            y = int.Parse(inp[1]);
        }
    }

    //Stores a view for the gameboard to render
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

    //Stores a string associated with a colour, to be drawn in that colour by the text renderer
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

    //This struct takes in from a string array and parses all the information required to make an entity
    internal struct EntityData
    {
        #region Fields
        public string type;
        public string name;
        public char icon;
        public int drawPriority;
        public Coordinate position;
        public Inventory inventory;
        public int[] stats;
        public string description;
        #endregion

        public EntityData(string[] data)
        {
            #region Parsing
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
            #endregion
        }
    }

    //This struct parses and stores a type and the attribute data needed to make an item
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

    //This struct sessentially is a vector2, used for making movement code more readable
    internal struct MoveCommand
    {
        public int x;
        public int y;

        public MoveCommand(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}

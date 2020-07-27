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
}

using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class Wall : Entity
    {
        public Wall(char icon, Coordinate position) : base("Wall", position, icon, null) {
            Impassable = true;
        }

        public Wall(string nameJunk, Coordinate position, char icon, int drawPriorityJunk, Inventory inventoryJunk) : base("Wall", position, icon, null)
        {
            Impassable = true;
        }
    }
}

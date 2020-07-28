using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class Wall : Entity
    {
        new static int[] stats = new int[] {100, 100, 0, 0, 0, 0};
        public Wall(char icon, Coordinate position) : base("Wall", position, icon, null, Stats1) {
            Passable = false;
            Passive = false;
        }

        public Wall(string nameJunk, Coordinate position, char icon, int drawPriorityJunk, Inventory inventoryJunk, int[] stats) : base("Wall", position, icon, null, stats)
        {
            Passable = false;
            Passive = false;
        }

        public static int[] Stats1 { get => stats; set => stats = value; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class Wall : Entity
    {
        new static int[] stats = new int[] {100, 100, 0, 0, 0, 0};
        public Wall(char icon, Coordinate position, string description) : base("Wall", position, icon, null, Stats1, description) {
            Passable = false;
            Passive = false;
        }

        public Wall(string nameJunk, Coordinate position, char icon, int drawPriorityJunk, Inventory inventoryJunk, int[] stats, string description) : base("Wall", position, icon, null, stats, description)
        {
            Passable = false;
            Passive = false;
        }

        public static int[] Stats1 { get => stats; set => stats = value; }
    }
}

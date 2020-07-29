using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    abstract class Enemy : Entity
    {
        public override bool Aggressive { get; internal set; } = true;

        public Enemy(String name, Coordinate position, char icon, int drawPriority, Inventory inventory, int[] stats) : base(name, position, icon, drawPriority, inventory, stats) { Passive = false; }

        public Enemy(String name, Coordinate position, char icon, int drawPriority, int[] stats) : base(name, position, icon, drawPriority, stats) { Passive = false; }

        public Enemy(String name, Coordinate position, char icon, Inventory inventory, int[] stats) : base(name, position, icon, inventory, stats) { Passive = false; }

        public Enemy(String name, Coordinate position, char icon, int[] stats) : base(name, position, icon, stats) { Passive = false; }
    }
}

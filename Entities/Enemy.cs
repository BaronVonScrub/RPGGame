using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    abstract class Enemy : Entity
    {
        public override bool Aggressive { get; internal set; } = true;

        public Enemy(String name, Coordinate position, char icon, int drawPriority, Inventory inventory, int[] stats, string description) : base(name, position, icon, drawPriority, inventory, stats, description) { Passive = false; }

        public Enemy(String name, Coordinate position, char icon, int drawPriority, int[] stats, string description) : base(name, position, icon, drawPriority, stats, description) { Passive = false; }

        public Enemy(String name, Coordinate position, char icon, Inventory inventory, int[] stats, string description) : base(name, position, icon, inventory, stats, description) { Passive = false; }

        public Enemy(String name, Coordinate position, char icon, int[] stats, string description) : base(name, position, icon, stats, description) { Passive = false; }
    }
}

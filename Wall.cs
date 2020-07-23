using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class Wall : Entity
    {
        public Wall(char icon, Coordinate position) : base("Wall", position, icon, null) {
        }
    }
}

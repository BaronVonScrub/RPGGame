namespace RPGGame
{
    internal class Wall : Entity
    {
        //Custom stats the same for all walls
        public static new int[] Stats { get; set; } = new int[] { 100, 100, 0, 0, 0, 0 };

        //Shorthand constructor
        public Wall(char icon, Coordinate position, string description) : base("Wall", position, icon, null, Stats, description)
        {
            Passable = false;
            Passive = false;
        }

        //Long form constructor if needed
        public Wall( Coordinate position, char icon, int drawPriorityJunk, Inventory inventoryJunk, int[] stats, string description) : base("Wall", position, icon, null, stats, description)
        {
            Passable = false;
            Passive = false;
        }


    }
}

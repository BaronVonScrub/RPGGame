namespace RPGGame
{
    internal abstract class Enemy : Entity
    {
        //Overrides the default aggressive stance to be true
        public override bool Aggressive { get; internal set; } = true;

        //Overrides the default passive inventory stance to be true (But you'll only get the chance to access it when it's already dead!)
        public override bool Passive { get; internal set; } = true;

        //Basic inherited constructor
        public Enemy(string name, Coordinate position, char icon, int drawPriority, Inventory inventory, int[] stats, string description) : base(name, position, icon, drawPriority, inventory, stats, description) { }
    }
}

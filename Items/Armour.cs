namespace RPGGame
{
    internal class Armour : Item
    {
        public Armour(string inputData) : base(inputData) { }

        public override string[] MustHave { get; set; } = new string[] { "defenceModifier", "armourModifier", "value", "name", "equipped" };
    }
}

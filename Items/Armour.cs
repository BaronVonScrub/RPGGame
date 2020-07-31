namespace RPGGame
{
    internal class Armour : Item
    {
        //Sets MustHaves (See Item)
        public override string[] MustHave { get; set; } = new string[] { "defenceModifier", "armourModifier", "value", "name", "equipped" };

        public Armour(string inputData) : base(inputData) { }
    }
}

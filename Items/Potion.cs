namespace RPGGame
{
    internal class Potion : Item
    {
        //Sets MustHaves (See Item)
        public override string[] MustHave { get; set; } = new string[] { "value", "name" };
        public Potion(string inputData) : base(inputData) { }
    }
}

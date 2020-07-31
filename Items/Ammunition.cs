namespace RPGGame
{
    internal class Ammunition : Item
    {
        //Sets MustHaves (See Item)
        public override string[] MustHave { get; set; } = new string[] { "value", "amount", "name" };
        public Ammunition(string inputData) : base(inputData) { }
    }
}

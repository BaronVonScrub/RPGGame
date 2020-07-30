namespace RPGGame
{
    internal class Ammunition : Item
    {
        public override string[] MustHave { get; set; } = new string[] { "value", "amount", "name" };
        public Ammunition(string inputData) : base(inputData) { }
    }
}

namespace RPGGame
{
    internal class Miscellaneous : Item
    {
        public override string[] MustHave { get; set; } = new string[] { "value", "name" };
        public Miscellaneous(string inputData) : base(inputData) { }
    }
}

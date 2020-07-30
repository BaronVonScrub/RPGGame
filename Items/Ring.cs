namespace RPGGame
{
    internal class Ring : Item
    {
        public override string[] MustHave { get; set; } = new string[] { "value", "name", "equipped" };
        public Ring(string inputData) : base(inputData) { }
    }
}

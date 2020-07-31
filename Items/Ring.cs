namespace RPGGame
{
    internal class Ring : Item
    {
        //Sets MustHaves (See Item)
        public override string[] MustHave { get; set; } = new string[] { "value", "name", "equipped" };
        public Ring(string inputData) : base(inputData) { }
    }
}

namespace RPGGame
{
    internal class Miscellaneous : Item
    {
        //Sets MustHaves (See Item)
        public override string[] MustHave { get; set; } = new string[] { "value", "name" };
        public Miscellaneous(string inputData) : base(inputData) { }
    }
}

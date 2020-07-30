namespace RPGGame
{
    internal class Weapon : Item
    {
        public int slotsRequired;
        public override string[] MustHave { get; set; } = new string[] { "attackModifier", "damageModifier", "maxRange", "minRange", "slotsNeeded", "value", "name", "equipped" };
        public Weapon(string inputData) : base(inputData) => slotsRequired = int.Parse(itemData["slotsNeeded"]);
    }
}

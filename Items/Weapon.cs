namespace RPGGame
{
    internal class Weapon : Item
    {
        //SlotsRequired directly implemented for easy access.
        public int slotsRequired;

        //Sets MustHaves (See Item)
        public override string[] MustHave { get; set; } = new string[] { "attackModifier", "damageModifier", "maxRange", "minRange", "slotsNeeded", "value", "name", "equipped" };

        //Constructor sets slotsrequred from itemData
        public Weapon(string inputData) : base(inputData) => slotsRequired = int.Parse(itemData["slotsNeeded"]);
    }
}

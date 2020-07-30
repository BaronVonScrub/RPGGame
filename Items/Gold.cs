using static RPGGame.TextManager;

namespace RPGGame
{
    internal class Gold : Item
    {
        private int amount = 0;
        private new readonly bool equipped = false;

        public override string[] MustHave { get; set; } = new string[] { "amount" };
        public int Amount { get => amount; set => amount = value; }

        public Gold(string inputData) : base(inputData)
        {
            Name = "Gold";
            Amount = int.Parse(itemData["amount"]);
            Value = Amount;
        }

        public Gold(int inputData) : base()
        {
            ForceSet("name", "Gold");
            ForceSet("value", inputData.ToString());
            ForceSet("amount", inputData.ToString());
            ForceSet("equipped", equipped.ToString());
            ForceSet("type", GetType().Name);
            Amount = inputData;
            Name = "Gold";
        }

        public override void Examine()
        {
            WriteLine(Name);
            WriteLine("Value : " + Amount);
        }
    }
}

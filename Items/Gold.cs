using static RPGGame.TextManager;

namespace RPGGame
{
    internal class Gold : Item
    {
        //Locks equipped to false
        private new readonly bool equipped = false;

        //Sets MustHaves (See Item)
        public override string[] MustHave { get; set; } = new string[] { "amount" };
        public int Amount { get; set; } = 0;                                            //Amount of gold

        public Gold(string inputData) : base(inputData)                                 //Creates gold via loaded inputData
        {
            Name = "Gold";
            Amount = int.Parse(itemData["amount"]);
            Value = Amount;
        }

        public Gold(int inputData) : base()                                             //Creates gold numerically (Used for merging gold items)
        {
            ForceSet("name", "Gold");                                                   //These values are all hard set.
            ForceSet("value", inputData.ToString());
            ForceSet("amount", inputData.ToString());
            ForceSet("equipped", equipped.ToString());
            ForceSet("type", GetType().Name);
            Amount = inputData;
            Name = "Gold";                                                              //
        }

        //Custom examine method
        public override void Examine()
        {
            WriteLine(Name);
            WriteLine("Value : " + Amount);
        }
    }
}

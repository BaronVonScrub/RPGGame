using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using static RPGGame.TextTool;

namespace RPGGame
{
    class Gold : Item
    {
        private int amount = 0;
        new readonly Boolean equipped = false;

        public override string[] MustHave { get; set; } = new string[] { "amount"};
        public int Amount { get => amount; set => amount = value; }

        public Gold(string inputData) : base(inputData)                         
        {   
            Name = "Gold";                                                      
            Amount = Int32.Parse(itemData["amount"]);                           
            Value = Amount;
        }

        public Gold(int inputData) : base()                            
        {
            ForceSet("name", "Gold");
            ForceSet("value", inputData.ToString());
            ForceSet("amount", inputData.ToString());
            ForceSet("equipped", equipped.ToString());
            ForceSet("type", this.GetType().Name);
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

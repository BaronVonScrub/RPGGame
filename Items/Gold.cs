using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace RPGGame
{
    class Gold : Item
    {
        private int amount = 0;
        new readonly Boolean equipped = false;

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
            ForceSet("equipped", "false");
            ForceSet("type", this.GetType().Name);
            amount = inputData;
        }
    }
}

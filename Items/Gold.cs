using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace RPGGame
{
    class Gold : Item
    {
        public int amount = 0;                                                  //Amount of gold stored in the item
        public Gold(string inputData) : base(inputData)                         //Constructor called from string, runs base constructor
        {
            if (itemData.ContainsKey("name"))                                   //Strip any given name
                itemData.Remove("name");

            itemData.Add("name", "Gold");                                       //Item attribute "name" is "Gold"
            name = "Gold";                                                      //Item name is "Gold"

            amount = Int32.Parse(itemData["amount"]);                           //Item attribute "amount" is amount
            value = amount;                                                     //Value is amount
        }

        public Gold(int inputData) : base(inputData)                            //Constructor called from integer, runs base constructor
        {
            if (itemData.ContainsKey("name"))                                   //Strip any given name
                itemData.Remove("name");

            itemData.Add("name", "Gold");                                       //Item attribute "name" is "Gold"
            name = "Gold";                                                      //Item name is "Gold"

            itemData.Add("amount", inputData.ToString());                       //Item attribute "amount" is specified by inputData
            amount = inputData;                                                 //Item amount is specified by inputData

            itemData.Add("value", inputData.ToString());                        //Item attribute "value" is specified by inputData
            value = inputData;                                                  //Item value is specified by inputData

            if (itemData.ContainsKey("type"))                                   //Strip any type given
                itemData.Remove("type");
            itemData.Add("type", GetType().Name);                               //Item type is "Gold"
        }
    }
}

using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace RPGGame
{
    class Gold : Item
    {
        public int amount = 0;                                                  
        public Gold(string inputData) : base(inputData)                         
        {                                     
            Name = "Gold";                                                      

            amount = Int32.Parse(itemData["amount"]);                           
            Value = amount;                                                     
        }

        public Gold(int inputData) : base(inputData)                            
        {
            if (itemData.ContainsKey("name"))                                   
                itemData.Remove("name");

            itemData.Add("name", "Gold");                                       
            Name = "Gold";                                                      

            itemData.Add("amount", inputData.ToString());                       
            amount = inputData;                                                 

            itemData.Add("value", inputData.ToString());                        
            Value = inputData;                                                  

            if (itemData.ContainsKey("type"))                                   
                itemData.Remove("type");
            itemData.Add("type", GetType().Name);                               
        }
    }
}

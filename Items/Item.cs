using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static RPGGame.TextTool;
using static RPGGame.GlobalVariables;
using System.Text;

namespace RPGGame
{
    abstract class Item                                                                                 //Parent item
    {
        Boolean equipped = false;
        public int value;                                                                               //The value of the item
        public string name = "Unnamed item";                                                            //The name of the item
        public SortedDictionary<String, String> itemData = new SortedDictionary<String, String>();      //The list of attributes of the item
        public Boolean Equipped {
            get
            {
                return equipped;
            }
            set
            {
                itemData["equipped"] = value.ToString();
            }
        }

        protected Item(string inputData)                                                                //Base constructor
        {
            foreach (Match match in Regex.Matches(inputData, AttFinder))                                //For each attribute found in inputData
                {
                String[] attData = match.Value.Split(":");                                              //Split it
                itemData.Add(attData[0],attData[1]);                                                    //Add the key and value to the attribute list
                }

            if (!itemData.ContainsKey("name"))                                                          //Must have name, or will be assigned "Unnamed Item"
                itemData.Add("name", name);
            else
                name = itemData["name"];

            if (itemData.ContainsKey("value"))                                                          //Must have value, or it will be assigned "0"      
                value = Int32.Parse(itemData["value"]);
            else
                itemData.Add("value", value.ToString());

            if (itemData.ContainsKey("equipped"))                                                          //Must have value, or it will be assigned "0"      
                equipped = Convert.ToBoolean(itemData["equipped"]);
            else
                itemData.Add("equipped", "false");

            if (!itemData.ContainsKey("type"))                                                          //Type is locked in                     
                itemData.Add("type",this.GetType().Name);
            else
            {
                itemData.Remove("type");
                itemData.Add("type", this.GetType().Name);
            }

        }

        protected Item(int inputData) { }

        virtual public String Look()
        {
            return name;                                                                       //Write the name of the item
        }

        public void Examine()                                                                           //Lists the item information
        {
            if (itemData.ContainsKey("name"))                                                           //Name first
                WriteLine(itemData["name"]);
            if (itemData.ContainsKey("type"))                                                           //Type second
                WriteLine("Type : "+itemData["type"]);
            if (itemData.ContainsKey("amount"))
                WriteLine("Amount : " + itemData["amount"]);                                       //Amount third
            if (itemData.ContainsKey("value"))
                WriteLine("value : " + itemData["value"]);                                         //Value fourth
            foreach (KeyValuePair<String,String> dat in itemData)                                       //For each attribute
            {
                if (dat.Key!="name"&&dat.Key!="type"&&dat.Key!="amount"&&dat.Key!="value")              //If it isn't one of those already listed
                    WriteLine(dat.Key + " : " + dat.Value);                                        //List it
            }
        }

    }
}

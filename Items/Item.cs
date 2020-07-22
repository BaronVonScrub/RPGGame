using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace RPGGame
{
    abstract class Item                                                                                 //Parent item
    {
        public int value;                                                                               //The value of the item
        public string name = "Unnamed item";                                                            //The name of the item
        public SortedDictionary<String, String> itemData = new SortedDictionary<String, String>();      //The list of attributes of the item
        static String attFinder = "(\\S+:[\\w\\s]+)(?=\\s|$)";                                          //The Regex format to parse attributes

        protected Item(string inputData)                                                                //Base constructor
        {
            foreach (Match match in Regex.Matches(inputData, attFinder))                                //For each attribute found in inputData
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
                TextTool.WriteLine(itemData["name"]);
            if (itemData.ContainsKey("type"))                                                           //Type second
                TextTool.WriteLine("Type : "+itemData["type"]);
            if (itemData.ContainsKey("amount"))
                TextTool.WriteLine("Amount : " + itemData["amount"]);                                       //Amount third
            if (itemData.ContainsKey("value"))
                TextTool.WriteLine("value : " + itemData["value"]);                                         //Value fourth
            foreach (KeyValuePair<String,String> dat in itemData)                                       //For each attribute
            {
                if (dat.Key!="name"&&dat.Key!="type"&&dat.Key!="amount"&&dat.Key!="value")              //If it isn't one of those already listed
                    TextTool.WriteLine(dat.Key + " : " + dat.Value);                                        //List it
            }
        }

    }
}

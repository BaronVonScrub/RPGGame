using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static RPGGame.TextTool;
using static RPGGame.GlobalVariables;
using System.Text;

namespace RPGGame
{
    abstract class Item                                                                                 
    {
        private Boolean equipped = false;
        private int val;
        private string name = "Unnamed item";
        public SortedDictionary<String, String> itemData = new SortedDictionary<String, String>();      
        
        public Boolean Equipped {
            get => equipped;
            set
            {
                equipped = value;
                if (itemData.ContainsKey("equipped"))
                    itemData.Remove("equipped");
                itemData.Add("equipped", value.ToString());
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                if (itemData.ContainsKey("name"))
                    itemData.Remove("name");
                itemData.Add("name", value);
            }
        }

        public int Value
        {
            get => val;
            set
            {
                val = value;
                if (itemData.ContainsKey("value"))
                    itemData.Remove("value");
                itemData.Add("value", value.ToString());
            }
        }

        protected Item(string inputData)                                                                
        {
            foreach (Match match in Regex.Matches(inputData, AttFinder))                                
                {
                String[] attData = match.Value.Split(":");                                              
                itemData.Add(attData[0],attData[1]);                                                    
                }

            if (!itemData.ContainsKey("name"))                                                          
                itemData.Add("name", Name);
            else
                Name = itemData["name"];

            if (itemData.ContainsKey("value"))                                                          
                Value = Int32.Parse(itemData["value"]);
            else
                itemData.Add("value", Value.ToString());

            if (itemData.ContainsKey("equipped"))                                                          
                equipped = Convert.ToBoolean(itemData["equipped"]);
            else
                itemData.Add("equipped", "false");

            if (!itemData.ContainsKey("type"))                                                          
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
            return Name;                                                                       
        }

        public void Examine()                                                                           
        {                                                      
            WriteLine(Name);
            if (itemData.ContainsKey("type"))                                                           
                WriteLine("Type : "+itemData["type"]);
            if (itemData.ContainsKey("amount"))
                WriteLine("Amount : " + itemData["amount"]);                                       
            if (itemData.ContainsKey("value"))
                WriteLine("value : " + itemData["value"]);                                         
            foreach (KeyValuePair<String,String> dat in itemData)                                       
            {
                if (dat.Key!="name"&&dat.Key!="type"&&dat.Key!="amount"&&dat.Key!="value")              
                    WriteLine(dat.Key + " : " + dat.Value);                                        
            }
        }

    }
}

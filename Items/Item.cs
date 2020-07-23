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
        Boolean equipped = false;
        public int value;                                                                               
        public string name = "Unnamed item";                                                            
        public SortedDictionary<String, String> itemData = new SortedDictionary<String, String>();      
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

        protected Item(string inputData)                                                                
        {
            foreach (Match match in Regex.Matches(inputData, AttFinder))                                
                {
                String[] attData = match.Value.Split(":");                                              
                itemData.Add(attData[0],attData[1]);                                                    
                }

            if (!itemData.ContainsKey("name"))                                                          
                itemData.Add("name", name);
            else
                name = itemData["name"];

            if (itemData.ContainsKey("value"))                                                          
                value = Int32.Parse(itemData["value"]);
            else
                itemData.Add("value", value.ToString());

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
            return name;                                                                       
        }

        public void Examine()                                                                           
        {
            if (itemData.ContainsKey("name"))                                                           
                WriteLine(itemData["name"]);
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

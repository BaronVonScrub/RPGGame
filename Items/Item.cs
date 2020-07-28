﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static RPGGame.TextTool;
using static RPGGame.ConstantVariables;

namespace RPGGame
{
    abstract class Item
    {
        protected Boolean equipped = false;
        protected int val;
        protected string name = "";
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

        abstract public string[] MustHave { get; set; }

        protected Item() { }

        protected Item(string inputData)                                                                
        {
            foreach (Match match in Regex.Matches(inputData, AttFinder))                                
            {
                String[] attData = match.Value.Split(":");
                itemData.Add(attData[0],attData[1]);
            }

            Guarantee();

            AttributeSet("name",ref name);
            AttributeSet("value", ref val);
            AttributeSet("equipped", ref equipped);
            ForceSet("type", this.GetType().Name);
        }

        public void Guarantee()
        {
            foreach (string att in MustHave)
                AttributeSet(att);
        }

        virtual public string Look()
        {
            if (Equipped == true)
                return Name + " (Equipped)";
            return Name;                                                                       
        }

        protected void ForceSet(String key, string value)
        {
            if (itemData.ContainsKey(key))
                itemData.Remove(key);
            itemData.Add(key, value);
        }

        protected void AttributeSet(String key, ref string refVal) {
            if (itemData.ContainsKey(key))
                refVal = itemData[key];
            else
                itemData[key] = refVal;
        }

        protected void AttributeSet(String key, ref Boolean refVal)
        {
            if (itemData.ContainsKey(key))
                refVal = Boolean.Parse(itemData[key]);
            else
                itemData[key] = refVal.ToString();
        }

        protected void AttributeSet(String key, ref int refVal)
        {
            if (itemData.ContainsKey(key))
                refVal = Int32.Parse(itemData[key]);
            else
                itemData[key] = refVal.ToString();
        }

        protected void AttributeSet(String key)
        {
            if (!itemData.ContainsKey(key))
                itemData[key] = 0.ToString();
        }

        virtual public void Examine()                                                                           
        {                                                      
            WriteLine(Name);                                                     
            WriteLine("Type : "+itemData["type"]);
            if (itemData.ContainsKey("amount"))
                WriteLine("Amount : " + itemData["amount"]);                                       
            WriteLine("Value : " + itemData["value"]);       
            
            foreach (KeyValuePair<String,String> dat in itemData)                                       
            {
                if (dat.Key!="name"&&dat.Key!="type"&&dat.Key!="amount"&&dat.Key!="value")              
                    WriteLine(ToTitleCase(dat.Key) + " : " + dat.Value);                                        
            }
        }

    }
}

using System.Collections.Generic;
using System.Text.RegularExpressions;
using static RPGGame.ConstantVariables;
using static RPGGame.TextManager;

namespace RPGGame
{
    internal abstract class Item
    {
        protected bool equipped = false;
        protected int val;
        protected string name = "";
        public SortedDictionary<string, string> itemData = new SortedDictionary<string, string>();

        public bool Equipped
        {
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

        public abstract string[] MustHave { get; set; }

        protected Item() { }
        public int Get(string key) => int.Parse(itemData[key]);
        protected Item(string inputData)
        {
            foreach (Match match in Regex.Matches(inputData, AttFinder))
            {
                string[] attData = match.Value.Split(":");
                itemData.Add(attData[0], attData[1]);
            }

            Guarantee();

            AttributeSet("name", ref name);
            AttributeSet("value", ref val);
            AttributeSet("equipped", ref equipped);
            ForceSet("type", GetType().Name);
        }

        public void Guarantee()
        {
            foreach (string att in MustHave)
                AttributeSet(att);
        }

        public virtual string Look()
        {
            if (Equipped == true)
                return Name + " (Equipped)";
            return Name;
        }

        protected void ForceSet(string key, string value)
        {
            if (itemData.ContainsKey(key))
                itemData.Remove(key);
            itemData.Add(key, value);
        }

        protected void AttributeSet(string key, ref string refVal)
        {
            if (itemData.ContainsKey(key))
                refVal = itemData[key];
            else
                itemData[key] = refVal;
        }

        protected void AttributeSet(string key, ref bool refVal)
        {
            if (itemData.ContainsKey(key))
                refVal = bool.Parse(itemData[key]);
            else
                itemData[key] = refVal.ToString();
        }

        protected void AttributeSet(string key, ref int refVal)
        {
            if (itemData.ContainsKey(key))
                refVal = int.Parse(itemData[key]);
            else
                itemData[key] = refVal.ToString();
        }

        protected void AttributeSet(string key)
        {
            if (!itemData.ContainsKey(key))
            {
                if (key == "equipped")
                {
                    itemData[key] = "false";
                    return;
                }
                itemData[key] = 0.ToString();
            }
        }

        public virtual void Examine()
        {
            WriteLine(Name);
            WriteLine("Type : " + itemData["type"]);
            if (itemData.ContainsKey("amount"))
                WriteLine("Amount : " + itemData["amount"]);
            WriteLine("Value : " + itemData["value"]);

            foreach (KeyValuePair<string, string> dat in itemData)
            {
                if (dat.Key != "name" && dat.Key != "type" && dat.Key != "amount" && dat.Key != "value")
                    WriteLine(ToTitleCase(dat.Key) + " : " + dat.Value);
            }
        }

    }
}

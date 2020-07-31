using System.Collections.Generic;
using System.Text.RegularExpressions;
using static RPGGame.GlobalConstants;
using static RPGGame.TextManager;

namespace RPGGame
{
    internal abstract class Item
    {
        protected bool equipped = false;                                                                    //Is the item equipped?
        protected int val;                                                                                  //Value of the item (Value is a keyword in C#, so shortened to val)
        protected string name = "";                                                                         //Name for easy reference
        public SortedDictionary<string, string> itemData = new SortedDictionary<string, string>();          //Stores the item data
        public abstract string[] MustHave { get; set; }                                                     //Overwritten by children by a set of attributes the child class MUST have. 

        //These properties have accessors that ensure that whenever the property is set, it is updated in the itemData automatically
        #region Equip, Name, Value properties
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
        #endregion

        #region Construction
        protected Item() { }

        //Create the item from an inputdata string (usually from file)
        protected Item(string inputData)
        {
            foreach (Match match in Regex.Matches(inputData, AttFinder))                //For each attribute in the input data
            {
                string[] attData = match.Value.Split(":");                              //Split it at the colon
                itemData.Add(attData[0], attData[1]);                                   //Add the components of the split to itemdata as key and value respectively
            }

            Guarantee();                                                                //Ensures that the MustHave attributes for the child class are respected

            AttributeSet("name", ref name);                                             //Sets the name attribute if it isn't there, or property if it is.
            AttributeSet("value", ref val);                                             //Sets the value attribute if it isn't there, or property if it is.
            AttributeSet("equipped", ref equipped);                                     //Sets the equipped attribute if it isn't there, or property if it is.
            ForceSet("type", GetType().Name);                                           //Foribly overwrites the type attribute to the item class name.
        }

        //Forcibly overwrites a key in itemdata
        protected void ForceSet(string key, string value)
        {
            if (itemData.ContainsKey(key))
                itemData.Remove(key);
            itemData.Add(key, value);
        }

        //Sets the string attribute if it doesn't exist, or the reference string property if it does.
        protected void AttributeSet(string key, ref string refVal)
        {
            if (itemData.ContainsKey(key))
                refVal = itemData[key];
            else
                itemData[key] = refVal;
        }

        //Sets the string attribute if it doesn't exist, or the reference boolean property if it does.
        protected void AttributeSet(string key, ref bool refVal)
        {
            if (itemData.ContainsKey(key))
                refVal = bool.Parse(itemData[key]);
            else
                itemData[key] = refVal.ToString();
        }

        //Sets the string attribute if it doesn't exist, or the reference int property if it does.
        protected void AttributeSet(string key, ref int refVal)
        {
            if (itemData.ContainsKey(key))
                refVal = int.Parse(itemData[key]);
            else
                itemData[key] = refVal.ToString();
        }

        //Used to guarantee musthaves - Sets the key to a default int.ToString() if it doesn't exist - unless it is "equipped", in which case it is a boolean.ToString()
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
        #endregion

        //This method guarantees the MustHave attributes of any child classes
        public void Guarantee()
        {
            foreach (string att in MustHave)
                AttributeSet(att);
        }

        //Allow the easy getting of an int itemdata by method
        public int Get(string key) => int.Parse(itemData[key]);

        //Outputs the name of the item, + equipped if it is equipped.
        public virtual string Look()
        {
            if (Equipped == true)
                return Name + " (Equipped)";
            return Name;
        }

       //Examines the item and all its stats.
        public virtual void Examine()
        {
            WriteLine(Name);                                                                                    //List name
            WriteLine("Type : " + itemData["type"]);                                                            //List type
            if (itemData.ContainsKey("amount"))
                WriteLine("Amount : " + itemData["amount"]);                                                    //List amount if appropriate
            WriteLine("Value : " + itemData["value"]);                                                          //List value

            foreach (KeyValuePair<string, string> dat in itemData)                                              //List all others
            {
                if (dat.Key != "name" && dat.Key != "type" && dat.Key != "amount" && dat.Key != "value")
                    WriteLine(ToTitleCase(dat.Key) + " : " + dat.Value);
            }
        }

    }
}

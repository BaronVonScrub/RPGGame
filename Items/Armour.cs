using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class Armour : Item
    {
        public Armour(string inputData) : base(inputData) { }

        public override string[] MustHave { get; set; } = new string[] {"defenceModifier","armourModifier", "value", "name", "equipped"};
    }
}

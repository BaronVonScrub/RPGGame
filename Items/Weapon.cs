using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class Weapon : Item
    {
        public int slotsRequired;
        public override string[] MustHave { get; set; } = new string[] { "attackModifier", "damageModifier", "maxRange", "minRange", "slotsNeeded", "value", "name", "equipped" };
        public Weapon(string inputData) : base(inputData) {
            slotsRequired = Int32.Parse(itemData["slotsNeeded"]);
        }
    }
}

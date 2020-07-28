using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class Weapon : Item
    {
        public override string[] MustHave { get; set; } = new string[] { "accuracyModifier", "damageModifier", "maxRange", "minRange", "slotsNeeded", "value", "name", "equipped" };
        public Weapon(string inputData) : base(inputData) {}
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class Potion : Item
    {
        public override string[] MustHave { get; set; } = new string[] { "value", "name" };
        public Potion(string inputData) : base(inputData) { }
    }
}

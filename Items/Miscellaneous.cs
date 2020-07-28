using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class Miscellaneous : Item
    {
        public override string[] MustHave { get; set; } = new string[] { "value", "name" };
        public Miscellaneous(string inputData) : base(inputData) { }
    }
}

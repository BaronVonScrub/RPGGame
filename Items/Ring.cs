using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class Ring : Item
    {
        public override string[] MustHave { get; set; } = new string[] { "value", "name", "equipped" };
        public Ring(string inputData) : base(inputData) { }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class Ammunition : Item
    {
        public override string[] MustHave { get; set; } = new string[] { "value","amount", "name"};
        public Ammunition(string inputData) : base(inputData) { }
    }
}

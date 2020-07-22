using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class Weapon : Item
    {
        int attackModifier = 0;                                                     //Has required attribute: attackModifier
        public Weapon(string inputData) : base(inputData) {
            attackModifier = Int32.Parse(itemData["attackModifier"]);               //Has required attribute: attackModifier
        }

    }
}

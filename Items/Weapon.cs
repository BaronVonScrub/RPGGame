using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class Weapon : Item
    {
        int attackModifier = 0;                                                     
        public Weapon(string inputData) : base(inputData) {
            attackModifier = Int32.Parse(itemData["attackModifier"]);               
        }

    }
}

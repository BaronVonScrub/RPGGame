using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame.Entities
{
    class Imp : Enemy
    {
        private static Dictionary<String, Item[]> equiptory = new Dictionary<String, Item[]>()
        {
            { "Weapon",new Item[1]},
            { "Ring", new Item[10]},
            { "Armour", new Item[0] },
            { "Miscellaneous", new Item[5] }
        };

    }
}

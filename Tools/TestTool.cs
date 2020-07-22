using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    static class TestTool
    {
        public static List<string> testList = new List<string>                                                        //List of test commands
            {
            "HELP",
            "LOOK INVENTORY",
            "LOOK MERCHANT",
            "EXAMINE Axe",
            "BUY Axe",
            "LOOK INVENTORY",
            "LOOK MERCHANT",
            "SELL Axe",
            "LOOK INVENTORY",
            "LOOK MERCHANT",
            "RENAME INVENTORY Bow Longbow",
            "LOOK INVENTORY",
            "EXAMINE Longbow",
            "EXAMINE Bow",
            "EXAMINE MERCHANT Axe",
            "GIVE ME GOOD GRADES",
            "ADD MERCHANT WEAPON name:Rapier attackModifier:5 value:35",
            "LOOK MERCHANT",
            "EXAMINE Rapier",
            "REMOVE Rapier",
            "LOOK MERCHANT",
            "QUIT"
            };
    }
}

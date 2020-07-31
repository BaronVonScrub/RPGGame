using System;
using System.Collections.Generic;
using static RPGGame.GlobalConstants;
using static RPGGame.GlobalVariables;
using static RPGGame.ImportExportManager;
using static RPGGame.ParseManager;
using static RPGGame.TextManager;

namespace RPGGame
{
    internal static class InventoryManager
    {
        public static int InventoryNum { get; set; } = 0;

        public static void Initialize() => ImportInventories("Inventories.dat");

        public static void TestInitialize() => ImportInventories("TestInventories.dat");

        public static Inventory GenerateInv()
        {
            var r = new Random();
            int i = 0;
            int level = 0;
            do
            {
                level++;
                i = r.Next(5);
            }
            while (i != 1);

            string inventName;
            do
            {
                InventoryNum++;
                inventName = "Inventory" + InventoryNum.ToString();
            }
            while (Inventories.Exists(x => x.name == inventName));

            var newInv = new Inventory(inventName, new List<Item>());

            level = Math.Min(level, ItemTable.Length);
            int num = r.Next(level);

            for (int j = 0; j < level; j++)
                newInv.inventData.Add(ItemTable[r.Next(level)]);

            Inventories.Add(newInv);
            return newInv;
        }

        public static Item RemoveNoLog(bool bypass)
        {
            if (!SuperStatus && bypass == false)
            {
                WriteLine("You do not have super access!");
                return null;
            }

            Target = GetTarget();
            string data = Strip(Input);

            if (GetCurrentInventoryList(Target) == null)
            {
                WriteLine("Inventory not found!");
                return null;
            }
            Item remove = GetCurrentInventoryList(Target).Find(x => data.ToUpper() == x.Name.ToUpper());

            if (remove != null)
            {
                GetCurrentInventoryList(Target).Remove(remove);
                return remove;
            }
            else
                WriteLine("Item not found!");
            return null;
        }

        public static Item Grab(Entity from, Entity to)
        {

            if (!(InventoryIsAccessible(to) && InventoryIsAccessible(from)))
            {
                WriteLine("Other inventory is not accessible.");
                return null;
            }

            if (to == null || from == null)
            {
                WriteLine("Cannot trade with a null inventory!");
                return null;
            }

            if (to == from)
            {
                WriteLine("Cannot trade with yourself!");
                return null;
            }

            Target = from;
            Item moveItem = RemoveNoLog(true);

            if (moveItem == null)
                return null;

            return moveItem;
        }

        public static bool Trade(Entity from, Entity to)
        {
            int value;

            if (from == null || to == null)
                return false;

            Item moveItem = Grab(from, to);

            if (moveItem == null)
                return false;

            if (moveItem.Equipped == true)
            {
                WriteLine("Item cannot be traded while equipped!");
                from.inventory.inventData.Add(moveItem);
                return false;
            }

            value = moveItem.Value;

            if (moveItem.GetType().Name == "Gold")
            {
                WriteLine("Can't trade gold!");
                from.inventory.inventData.Add(moveItem);
                return false;
            }

            if (GetGold(to) < value)
            {
                WriteLine("Not enough gold!");
                from.inventory.inventData.Add(moveItem);
                return false;
            }

            from.inventory.inventData.Add(new Gold(value));
            GoldMerge(from);

            to.inventory.inventData.Add(moveItem);
            to.inventory.inventData.Add(new Gold(-1 * value));
            GoldMerge(to);

            return true;
        }

        public static List<Inventory> GetInventories(List<Entity> entList)
        {
            var invList = new List<Inventory>();
            foreach (Entity ent in entList.FindAll(x => x.inventory != null))
            {
                invList.Add(ent.inventory);
            }
            return invList;
        }

        public static int AlphabeticalByName(Item a, Item b) => a.Name.CompareTo(b.Name);

        public static List<Item> GetCurrentInventoryList(Entity ent)
        {
            Inventory temp = GetInventory(ent);
            if (temp == null)
                return null;
            return temp.inventData;
        }

        public static List<Item> GetCurrentInventoryList(string invName)
        {
            Inventory temp = GetInventory(invName);
            if (temp == null)
                return null;
            return temp.inventData;
        }

        public static void GoldMerge(Entity ent)
        {
            int amount = 0;

            foreach (Gold gold in GetInventory(ent).inventData.FindAll(x => x.GetType().Name == "Gold"))
                amount += gold.Amount;

            ent.inventory.inventData.RemoveAll(x => x.GetType().Name == "Gold");

            if (amount != 0)
                ent.inventory.inventData.Add(new Gold(amount));
        }

        public static int GetGold(Entity ent)
        {
            GoldMerge(ent);
            int amount = 0;
            if (!GetInventory(ent).inventData.Exists(x => x.GetType().Name == "Gold"))
                return amount;
            amount = (GetInventory(ent).inventData.Find(x => x.GetType().Name == "Gold") as Gold).Amount;
            return amount;
        }

        public static Inventory GetInventory(string invName)
        {
            Inventory inv = Inventories.Find(x => x.name == invName);
            if (inv == null)
                return new Inventory(invName);
            return inv;
        }
        public static Inventory GetInventory(Entity ent)
        {
            if (ent == null)
                return null;
            Inventory inv = Inventories.Find(x => x.name == ent.inventory.name);
            if (inv == null)
                return new Inventory(ent.Name);
            return inv;
        }


        public static bool InventoryIsAccessible(Entity ent)
        {
            if (ent == null)
                return false;
            if (ent.inventory == null)
                return false;
            Inventory inv = GetLocalInventories().Find(x => x.name == ent.inventory.name);
            if (inv == null)
                return false;
            return true;
        }

        public static List<Inventory> GetLocalInventories() => GetInventories(MainBoard.GetFromBoard(Player.position));
    }
}

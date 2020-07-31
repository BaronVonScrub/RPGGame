using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static RPGGame.GlobalVariables;
using static RPGGame.ImportExportManager;
using static RPGGame.InventoryManager;
using static RPGGame.ParseManager;
using static RPGGame.TextManager;

namespace RPGGame
{
    internal static class EntityManager
    {
        private static int EntityNum { get; set; } = 0;

        public static void Initialize() => ImportEntities("Entities.dat");

        public static void TestInitialize() => ImportEntities("TestEntities.dat");

        public static bool UnequipFromTargetByName(Entity target, string inp)
        {
            string itemName = Strip(inp);
            Item item = target.inventory.GetItem(itemName);

            if (item != null)
            {
                bool successful = UnequipFromTargetByItem(target, item);
                if (successful)
                    WriteLine("Unequipped " + item.Name + "!");
                return successful;
            }

            WriteLine("Item not found!");
            return false;
        }

        public static bool UnequipFromTargetByItem(Entity target, Item item) => target.UnequipByItem(item);

        public static bool EquipToTargetByItem(Entity target, Item item)
        {
            if (item.Equipped == true)
            {
                WriteLine("That item is already equipped!");
                return false;
            }

            string itemType = item.GetType().Name;

            if (!target.Equiptory.ContainsKey(itemType))
            {
                WriteLine("Can't equip that!");
                return false;
            }

            int slotsRequired = 1;
            if (itemType == "Weapon")
                slotsRequired = (item as Weapon).slotsRequired;

            if (target.Equiptory[itemType].ToList().FindAll(x => x == null).Count < slotsRequired)
            {
                WriteLine("Not enough slots to equip " + item.Name + "!");
                item.Equipped = false;
                return false;
            }

            item.Equipped = true;

            for (int j = 0; j < slotsRequired; j++)
                for (int i = 0; i < target.Equiptory[itemType].Length; i++)
                    if (target.Equiptory[itemType][i] == null)
                    {
                        target.Equiptory[itemType][i] = item;
                        break;
                    }
            return true;
        }

        public static bool EquipToTargetByName(Entity target, string inp)
        {
            string itemName = Strip(inp);
            Item item = target.inventory.GetItem(itemName);

            if (item != null)
            {
                bool successful = EquipToTargetByItem(target, item);
                if (successful)
                    WriteLine("Equipped " + item.Name + "!");
                return successful;
            }

            WriteLine("Item not found!");
            return false;
        }

        public static Entity GetEntity(string entName)
        {
            Entity ent = null;
            foreach (List<Entity> entListCand in MainBoard.entityPos.Values.ToList())
            {
                ent = entListCand.Find(x => x.Name == entName);
                if (ent != null)
                    break;
            }
            return ent;
        }

        public static List<Entity> GetLocalEntities(Entity ent, GameBoard board) => board.GetFromBoard(ent.position);

        public static dynamic EntityCreate(string entType, string name, Coordinate position, char icon, int drawPriority, Inventory inventory, int[] stats, string description)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            Type currentType = currentAssembly.GetTypes().SingleOrDefault(t => t.Name == entType);
            return Activator.CreateInstance(currentType, name, position, icon, drawPriority, inventory, stats, description);
        }

        public static dynamic EntityCreate(EntityData inData)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            Type currentType = currentAssembly.GetTypes()
                                             .SingleOrDefault(t => t.Name == inData.type);
            return Activator.CreateInstance(currentType,
                                            inData.name,
                                            inData.position,
                                            inData.icon,
                                            inData.drawPriority,
                                            inData.inventory,
                                            inData.stats,
                                            inData.description); ;
        }

        public static void MonsterGen(MoveCommand direction, int dist)
        {
            Coordinate pos;
            var perp = new MoveCommand(direction.x == 1 ? 0 : 1, direction.y == 1 ? 0 : 1);
            int chancesPerSquare = (int)Math.Floor((float)dist / 10) - VisibleEnemies();
            var r = new Random();
            for (int i = -5; i < 6; i++)
            {
                pos = new Coordinate(Player.position.x + direction.x * 5 + perp.x * i, Player.position.y + direction.y * 5 + perp.y * i);

                bool make = false;
                for (int j = 0; j < chancesPerSquare; j++)
                    if (r.Next(0, 100) == 0)
                        make = true;

                if (make == true)
                    EnemyMake(pos);
            }
        }

        public static void CleanUp(GameBoard board)
        {

            var toBeRemoved = new List<Entity>();

            foreach (Coordinate coord in board.entityPos.Keys)
            {

                foreach (Entity ent in board.entityPos[coord])
                    if (ent.Dead == true && ent.inventory.inventData.Count == 0)
                        toBeRemoved.Add(ent);
            }

            foreach (Entity ent in toBeRemoved)
            {
                Inventories.Remove(ent.inventory);
                board.entityPos[ent.position].Remove(ent);
            }

            foreach (Coordinate coord in board.entityPos.Keys)
                if (board.entityPos[coord].Count == 0)
                    board.entityPos.Remove(coord);
        }

        private static int VisibleEnemies()
        {
            int num = 0;
            var coordList = new List<Coordinate>();
            var nearby = new List<Entity>();

            for (int i = -5; i < 6; i++)
                for (int j = -5; j < 6; j++)
                {
                    var coord = new Coordinate(Player.position.x + i, Player.position.y + j);
                    if (MainBoard.GetFromBoard(coord) != null)
                        foreach (Entity ent in MainBoard.GetFromBoard(coord))
                            nearby.Add(ent);
                }

            if (nearby.Exists(x => x.GetType().IsSubclassOf(typeof(Enemy))))
                num += nearby.FindAll(x => x.GetType().IsSubclassOf(typeof(Enemy))).Count;
            return num;
        }

        public static void EnemyMake(Coordinate pos)
        {
            var EnemyTypes = new List<Type>();

            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in types)
                if (type.IsSubclassOf(typeof(Enemy)))
                    EnemyTypes.Add(type);


            var r = new Random();
            string entType = EnemyTypes[r.Next(EnemyTypes.Count)].Name;
            string name;

            do
            {
                EntityNum++;
                name = entType + " " + EntityNum.ToString();
            }
            while (GetEntity(name) != null);



            char icon = (char)547;

            int drawPriority = 99;

            Inventory inventory = GenerateInv();

            int hp = 5 + r.Next(0, 20);

            int[] stats = { hp, hp, r.Next(1), 1 + r.Next(2), -1 };

            string description = "A fearsome enemy!";

            dynamic enemy = EntityCreate(entType, name, pos, icon, drawPriority, inventory, stats, description);

            GoldMerge(enemy);

            MainBoard.AddToBoard(enemy);
        }
    }
}

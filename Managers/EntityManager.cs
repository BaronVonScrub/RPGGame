using System;
using System.Collections.Generic;
using System.Linq;
using static RPGGame.GlobalVariables;
using static RPGGame.InventoryManager;
using static RPGGame.ImportExportTool;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RPGGame
{
    static class EntityManager
    {
        static int EntityNum { get; set; } = 0;
        public static void Initialize()
        {
            MainBoard = new GameBoard();
            ImportEntities();
            Player = GetEntity("Player");
        }

        private static Entity GetEntity(string entName)
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

        public static List<Entity> GetLocalEntities(Entity ent, GameBoard board)
        {
            return board.GetFromBoard(ent.position);
        }

        public static dynamic EntityCreate(string entType, string name, Coordinate position, Char icon, int drawPriority, Inventory inventory, int[] stats)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            var currentType = currentAssembly.GetTypes().SingleOrDefault(t => t.Name == entType);
            return Activator.CreateInstance(currentType, name, position, icon, drawPriority, inventory, stats);
        }

        public static dynamic EntityCreate(EntityData inData)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            var currentType = currentAssembly.GetTypes()
                                             .SingleOrDefault(t => t.Name == inData.type);
            return Activator.CreateInstance(currentType,
                                            inData.name,
                                            inData.position,
                                            inData.icon,
                                            inData.drawPriority,
                                            inData.inventory,
                                            inData.stats); ;
        }

        public static void MonsterGen(MoveCommand direction, int dist)
        {
            Coordinate pos;
            MoveCommand perp = new MoveCommand(direction.x == 1 ? 0 : 1, direction.y == 1 ? 0 : 1);
            int chancesPerSquare = (int)Math.Floor((float)dist / 10)-VisibleEnemies();
            Random r = new Random();
            Boolean make = false;
            for (int i = -5; i < 6; i++)
            {
                pos = new Coordinate(Player.position.x + direction.x * 5 + perp.x * i, Player.position.y + direction.y * 5 + perp.y * i);

                make = false;
                for (int j = 0; j < chancesPerSquare; j++)
                    if (r.Next(0, 100) == 0)
                        make = true;

                if (make == true)
                    EnemyMake(pos);
            }
        }

        public static void CleanUp(GameBoard board) {

            List<Entity> toBeRemoved = new List<Entity>();

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
            List<Coordinate> coordList = new List<Coordinate>();
            List<Entity> nearby = new List<Entity>();

            for (int i = -5; i < 6; i++)
                for (int j = -5; j < 6; j++)
                {
                    Coordinate coord = new Coordinate(Player.position.x + i, Player.position.y + j);
                    if (MainBoard.GetFromBoard(coord) != null)
                        foreach (Entity ent in MainBoard.GetFromBoard(coord))
                            nearby.Add(ent);
                }

            if (nearby.Exists(x => x.GetType().IsSubclassOf(typeof(Enemy))))
                num += nearby.FindAll(x => x.GetType().IsSubclassOf(typeof(Enemy))).Count;
            return num;
        }

        private static void EnemyMake(Coordinate pos)
        {
            List<Type> EnemyTypes = new List<Type>();

            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in types)
                if (type.IsSubclassOf(typeof(Enemy)))
                    EnemyTypes.Add(type);


            Random r = new Random();
            string entType = EnemyTypes[r.Next(EnemyTypes.Count)].Name;
            string name;

            do
            {
                EntityNum++;
                name = entType + " " + EntityNum.ToString();
            }
            while (GetEntity(name) != null);



            Char icon = (char)547;

            int drawPriority = 99;

            Inventory inventory = GenerateInv();

            int hp = 5+r.Next(0,20);

            int[] stats = { hp, hp, r.Next(1), 1+r.Next(2), -1 };

            dynamic enemy = EntityCreate(entType, name, pos, icon, drawPriority, inventory, stats);

            GoldMerge(enemy);

            MainBoard.AddToBoard(enemy);
        }
    }
}

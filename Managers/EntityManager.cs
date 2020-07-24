using System;
using System.Collections.Generic;
using System.Linq;
using static RPGGame.GlobalVariables;
using static RPGGame.InventoryManager;
using static RPGGame.ImportExportTool;
using System.Reflection;

namespace RPGGame
{
    static class EntityManager
    {
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

        public static List<Entity> GetLocalEntities()
        {
            return MainBoard.GetFromBoard(Player.position);
        }

        public static dynamic EntityFactoryReflection(string entType, string name, Coordinate position, Char icon, int drawPriority, Inventory inventory) {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            var currentType = currentAssembly.GetTypes().SingleOrDefault(t => t.Name == entType);
            return Activator.CreateInstance(currentType,name,position,icon,drawPriority,inventory);
        }
    }
}

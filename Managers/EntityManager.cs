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

        public static List<Entity> GetLocalEntities(Entity ent, GameBoard board)
        {
            return board.GetFromBoard(ent.position);
        }

        public static dynamic EntityCreate(string entType, string name, Coordinate position, Char icon, int drawPriority, Inventory inventory, int[] stats) {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            var currentType = currentAssembly.GetTypes().SingleOrDefault(t => t.Name == entType);
            return Activator.CreateInstance(currentType,name,position,icon,drawPriority,inventory,stats);
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
    }
}

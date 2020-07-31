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
        //Entity num incremented to give generated monsters unique inventory IDs
        private static int EntityNum { get; set; } = 0;

        //Initialization imports the entities saved on file
        public static void Initialize() => ImportEntities("Entities.dat");

        //Test initalization imports the entities saved in the test file
        public static void TestInitialize() => ImportEntities("TestEntities.dat");

        //Unequips a specified item (by name) from the specified target
        public static bool UnequipFromTargetByName(Entity target, string inp)
        {
            string itemName = Strip(inp);                                       //Get the noncommand data
            Item item = target.Inventory.GetItem(itemName);                     //Get the item from the inventory

            if (item != null)                                                   //If you find it
            {
                bool successful = UnequipFromTargetByItem(target, item);        //Attempt to unequip by target and item
                if (successful)
                    WriteLine("Unequipped " + item.Name + "!");
                return successful;                                              //Report and return if you do.
            }

            WriteLine("Item not found!");                                       //Otherwise report a failure
            return false;
        }

        //Alias method that calls the entity's unequip specified with the item
        public static bool UnequipFromTargetByItem(Entity target, Item item) => target.UnequipByItem(item);

        //Equip an item, target and item provided
        public static bool EquipToTargetByItem(Entity target, Item item)
        {
            if (item.Equipped == true)                                      //If it's equipped, fail
            {
                WriteLine("That item is already equipped!");
                return false;
            }

            string itemType = item.GetType().Name;                          //Get the type

            if (!target.Equiptory.ContainsKey(itemType))                    //If you have no slots for that type, fail
            {
                WriteLine("Can't equip that!");
                return false;
            }

            int slotsRequired = 1;
            if (itemType == "Weapon")
                slotsRequired = (item as Weapon).slotsRequired;             //Get the slots needed for the item

            if (target.Equiptory[itemType].ToList().FindAll(x => x == null).Count < slotsRequired)  //If you lack the slots
            {
                WriteLine("Not enough slots to equip " + item.Name + "!");  //Fail
                item.Equipped = false;
                return false;
            }

            item.Equipped = true;                                           //Otherwise, you're gonna pass!

            for (int j = 0; j < slotsRequired; j++)                         //For each slot required
                for (int i = 0; i < target.Equiptory[itemType].Length; i++) //Find a null slot in the appropriate equiptory branch and put it there
                    if (target.Equiptory[itemType][i] == null)
                    {
                        target.Equiptory[itemType][i] = item;
                        break;
                    }
            return true;                                                    //Return a success
        }

        //Equip an item, specified target, specified item by name
        public static bool EquipToTargetByName(Entity target, string inp)
        {
            string itemName = Strip(inp);                                   //Get noncommand data
            Item item = target.Inventory.GetItem(itemName);                 //Get the item from the inventory

            if (item != null)                                               //If you find it
            {
                bool successful = EquipToTargetByItem(target, item);        //Try to equip it
                if (successful)
                    WriteLine("Equipped " + item.Name + "!");
                return successful;                                          //Return if you did
            }

            WriteLine("Item not found!");
            return false;                                                   //Else fail
        }

        //Find an entity anywhere on the board, by name (Only finds first, so only usable for checking for free names, or finding uniques.)
        public static Entity GetEntity(string entName)
        {
            Entity ent = null;
            foreach (List<Entity> entListCand in MainBoard.entityPos.Values.ToList())       //For each list in each value of the mapped coordinates of the board
            {
                ent = entListCand.Find(x => x.Name == entName);                             //Find one by name
                if (ent != null)                                                            //If you find one, stop
                    break;
            }
            return ent;                                                                     //Return what you found, if anything
        }

        //Get the entities in the same square as given entity on given board
        public static List<Entity> GetLocalEntities(Entity ent, GameBoard board) => board.GetFromBoard(ent.position);

        //Create an entity via reflection, using manual paramaters.
        public static dynamic EntityCreate(string entType, string name, Coordinate position, char icon, int drawPriority, Inventory inventory, int[] stats, string description)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();                                                              //Get the current assembly
            Type currentType = currentAssembly.GetTypes().SingleOrDefault(t => t.Name == entType);                              //Find the type listed in the assmebly's type list
            return Activator.CreateInstance(currentType, name, position, icon, drawPriority, inventory, stats, description);    //Run its constructor with the provided parameters
        }

        //Create an entity via reflection, using an EntityData struct
        public static dynamic EntityCreate(EntityData inData)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();                                      //Get the current assembly
            Type currentType = currentAssembly.GetTypes()                                               
                                             .SingleOrDefault(t => t.Name == inData.type);              //Find the type listed in the assembly's type list
            return Activator.CreateInstance(currentType,                                                //Run its constructor with the EntityData's stored data
                                            inData.name,
                                            inData.position,
                                            inData.icon,
                                            inData.drawPriority,
                                            inData.inventory,
                                            inData.stats,
                                            inData.description); ;
        }

        //Method used to attempt to generate monsters, when the player moves.
        public static void MonsterGen(MoveCommand direction, int dist)
        {
            Coordinate pos;
            var perp = new MoveCommand(direction.x == 1 ? 0 : 1, direction.y == 1 ? 0 : 1);             //This creates a Movecommand perpendicular to the direction moved
            int chancesPerSquare = (int)Math.Floor((float)dist / 10) - VisibleEnemies();                //The chances are increased by distance from 0,0, reduced by exis
            var r = new Random();
            for (int i = -5; i < 6; i++)                                                                //For -5 to + 5
            {
                pos = new Coordinate(Player.position.x + direction.x * 5 + perp.x * i, Player.position.y + direction.y * 5 + perp.y * i);   //Make a coordinate perp to movement at map edge

                bool make = false;                                                                      //Reset the make command
                for (int j = 0; j < chancesPerSquare; j++)                                              //For each chance
                    if (r.Next(0, 100) == 0)                                                            //Roll a dice
                        make = true;                                                                    //Set make to true with a 1% chance

                if (make == true)
                    EnemyMake(pos);                                                                     //Make the enemy there if you rolled to
            }
        }

        //Cleans up dead entity with no loot, and coordinates that no longer have entities
        public static void CleanUp(GameBoard board)
        {

            var toBeRemoved = new List<Entity>();                                       //Irritatingly have to store them in a seperate list, as removing them during their own
                                                                                        //enumeration causes crashes

            foreach (Coordinate coord in board.entityPos.Keys)                          //For each coordinate stored
            {

                foreach (Entity ent in board.entityPos[coord])                          //For each entity in the coordinate
                    if (ent.Dead == true && ent.Inventory.inventData.Count == 0)        //If it's dead with an empty inventory
                        toBeRemoved.Add(ent);                                           //Queue it to be removed
            }

            foreach (Entity ent in toBeRemoved)                                         //Remove all entities queued to be removed, and their associated inventories
            {
                Inventories.Remove(ent.Inventory);
                board.entityPos[ent.position].Remove(ent);
            }

            foreach (Coordinate coord in board.entityPos.Keys)                          //For each stored coordinate, if it's got no entities, remove it.
                if (board.entityPos[coord].Count == 0)                                  //Wait, this shouldn't work due to the enumeration issue. FUTURE FIX.
                    board.entityPos.Remove(coord);
        }

        //Returns the number of enemies visible within the player's view. Just realised includes dead ones, so you could spawnproof an area by leaving corpses. OOPS!
        private static int VisibleEnemies()
        {
            int num = 0;
            var coordList = new List<Coordinate>();
            var nearby = new List<Entity>();

            for (int i = -5; i < 6; i++)
                for (int j = -5; j < 6; j++)
                {
                    var coord = new Coordinate(Player.position.x + i, Player.position.y + j);       //For each coordinate in the view
                    if (MainBoard.GetFromBoard(coord) != null)                                      //If there are entities there
                        foreach (Entity ent in MainBoard.GetFromBoard(coord))                       //Add each entity to the nearby list
                            nearby.Add(ent);
                }

            if (nearby.Exists(x => x.GetType().IsSubclassOf(typeof(Enemy))))                        //If there is one or more enemy in the nearby list
                num += nearby.FindAll(x => x.GetType().IsSubclassOf(typeof(Enemy))).Count;          //Count them and return that number
            return num;
        }

        //Creates a randomly generated enemy
        public static void EnemyMake(Coordinate pos)
        {
            var EnemyTypes = new List<Type>();                              //Will store potential enemy types

            Type[] types = Assembly.GetExecutingAssembly().GetTypes();      //Get thetypes from the current assembly
            foreach (Type type in types)
                if (type.IsSubclassOf(typeof(Enemy)))
                    EnemyTypes.Add(type);                                   //Add the enemies to the list


            var r = new Random();
            string entType = EnemyTypes[r.Next(EnemyTypes.Count)].Name;     //Get the name of a random enemy type
            string name;

            do
            {
                EntityNum++;
                name = entType + " " + EntityNum.ToString();                //Generate a unique name from the type + the incrementing entitynum
            }
            while (GetEntity(name) != null);

            char icon = (char)547;                                          //Set the icon

            int drawPriority = 99;                                          //Set the drawpriority

            Inventory inventory = GenerateInv();                            //Set the inventory to a randomly generated on

            int hp = 5 + r.Next(0, 20);                                     //Set a random hp between 5 and 25

            int[] stats = { hp, hp, r.Next(1), 1 + r.Next(2), -1 };         //Set some random stats

            string description = "A fearsome enemy!";                       //Set a generic description

            dynamic enemy = EntityCreate(entType, name, pos, icon, drawPriority, inventory, stats, description);    //Create the enemy from the data

            GoldMerge(enemy);                                               //Merge any multiple gold items in the enemy's inventory

            MainBoard.AddToBoard(enemy);                                    //Add the enemy to the board.
        }
    }
}

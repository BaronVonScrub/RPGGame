using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static RPGGame.CombatManager;
using static RPGGame.ConsoleManager;
using static RPGGame.GlobalConstants;
using static RPGGame.EntityManager;
using static RPGGame.GlobalVariables;
using static RPGGame.ImportExportManager;
using static RPGGame.InventoryManager;
using static RPGGame.MusicPlayer;
using static RPGGame.ParseManager;
using static RPGGame.TextManager;

namespace RPGGame
{
    internal class CommandManager
    {
        //This class handles all methods run by player commands - none of which take inputs.

        //Displays description of the listed entity
        public static void Look()
        {
            InventoryView = false;          //Disable inventory view
            Target = GetTarget();           //Get the target from input
            WriteLine(Target.Description);  //Output description
        }

        //Do nothing if the input is empty.
        public static void Empty()
        { }

        //Toggle the mute function of the musicplayer
        public static void MuteToggle() => Mute = !Mute;

        //Attempt to move the player
        public static void Move(MoveCommand direction)
        {
            InventoryView = false;          //Disable inventory view
            MapDraw = true;                 //Request a fresh map draw

            //Get a list of the entities at the goal position
            List<Entity> goalSquareEntities = MainBoard.GetFromBoard(new Coordinate(Player.position.x + direction.x, Player.position.y + direction.y));

            //If there is something impassable there, fail the move
            if (goalSquareEntities != null && goalSquareEntities.Exists(x => x.Passable == false))
            {
                WriteLine("Can't walk there!");
                return;
            }

            //If you fail a combat check (can't beat an aggressor in the square), fail the move.
            if (!CombatCheck(goalSquareEntities))
            {
                return;
            }

            //Remove the player from its current coordinates, update the position, readd it to the coordinate map.
            MainBoard.entityPos[Player.position].Remove(Player);
            Player.position.x += direction.x;
            Player.position.y += direction.y;
            MainBoard.AddToBoard(Player);

            //Call a cleanup function to remove any dead entities and their inventories if the inventories are empty, and clear their
            //coordinates from the map if there are no more entities there.
            CleanUp(MainBoard);

            //Attempt to generate monsters, likelihood increases with distance from (0,0) (Bugged to not do it in the right way)
            MonsterGen(direction, Player.DistanceFromCenter());

            //List the entities you see in your new square
            foreach (Entity ent in EntityManager.GetLocalEntities(Player, MainBoard).FindAll(x => (x.Name != "Player")))
                WriteLine("You see a " + ent.Name + ent.Status + "\b");
        }

        //Unequip by input defaults to removing by Player and Input
        public static void UnequipByInput() => UnequipFromTargetByName(Player, Input);

        //Equip by input defaults to adding by Player and Input
        public static void EquipByInput() => EquipToTargetByName(Player, Input);

        //Buy an item listed in input. Needs optimizing
        public static void Buy()
        {
            Entity other = GetTarget();                 //Get the target from input

            if (Trade(GetTarget(), Player))             //If you successfully trade the target item from input target entity to the player
                WriteLine("Item bought!");              //State it
            else
                WriteLine("Purchase failed!");          //Otherwise list failure

            Target = other;                             //Default current target to the other
        }

        //Basically just buying in reverse!
        public static void Sell()
        {
            Entity other = GetTarget();

            if (Trade(Player, GetTarget()))
                WriteLine("Item sold!");
            else
                WriteLine("Sale failed!");

            Target = other;
        }

        //Interact views the inventory of the target specified in input
        public static void Interact()
        {
            ViewInventory(GetTarget());
        }

        //LookAtMe views the player inventory
        public static void LookAtMe()
        {
            ViewInventory(Player);
        }

        //Exit manually leaves the inventory view
        public static void Exit()
        {
            if (InventoryView)
            {
                InventoryView = false;
                MapDraw = true;
                Redraw();
            }
            else
                WriteLine("Only usable when viewing an inventory!");
        }

        //Manual save
        public static void Save()
        {
            if (InternalTesting == true)
            {
                WriteLine("Save disabled during testing!");
                return;
            }

            ExportInventories();
            ExportEntities();
            WriteLine("Gamestate saved!");
        }

        //Examine the item listed in the input
        public static void Examine()
        {
            string data = Strip(Input);                     //Get non-command input
            if (GetCurrentInventoryList(Target) == null)    //If the target has no inventory
            {
                WriteLine("Inventory not found!");          //Say so
                return;                                     //End
            }

            //Find the item in the inventory, prioritising unequipped ones
            Item item = GetCurrentInventoryList(Target).Find(x => ((data.ToUpper() == x.Name.ToUpper()) && (x.Equipped == false)));
            if (item == null)
                item = GetCurrentInventoryList(Target).Find(x => (data == x.Name));

            //Escapes
            if (item == null)
            {
                WriteLine("Item not found!");
                return;
            }

            if (Target != Player && item.Equipped == true)
            {
                WriteLine("Can't examine while someone else has it equipped!");
                return;
            }
            //
            item.Examine();                                 //Run the item's Examine method
        }

        //Rename an item provided by input to something new
        public static void Rename()
        {
            Target = GetTarget();                           //Get the target inventory (You can change other peoples' to make dealing with duplicates easier)
            string data = Strip(Input);                     //Get the non-command input
            if (GetCurrentInventoryList(Target) == null)    //If there is no inventory
            {
                WriteLine("Inventory not found!");          //Say so
                return;                                     //End
            }

            Item item = GetCurrentInventoryList(Target).Find(x => data.Contains(x.Name));   //Find the item such that its name is stored in the input
            if (item != null)                                                               //If you find something
            {
                data = data.Replace(item.Name + " ", "");                                   //Remove the original name from input
                data = Regex.Replace(data, "^[\\s]+|[\\s]+$", "");                          //Strip whitespace
                item.Name = data;                                                           //Use the remaining input (No commands, no old name)
                WriteLine("Item renamed!");
            }
            else
                WriteLine("Item not found!");
        }

        //Take an item for free (Only works with passive inventories)
        public static void Take()
        {
            Target = GetTarget();       //Get target

            #region Escapes
            if (Target == null)
            {
                WriteLine("Target not found!");
                return;
            }

            if (Target.Passive == false)
            {
                WriteLine("They won't let you just take it!");
                return;
            }
            #endregion

            Item moveItem = Grab(Target, Player);       //Store the item and remove it from the initial inventory

            if (moveItem == null)                       //End if you found nothing
                return;

            WriteLine(moveItem.Name + " taken!");       //Else list it
            Player.Inventory.inventData.Add(moveItem);  //And add it to your inventory

            CleanUp(MainBoard);                         //Clean up in case of empty inventories

        }

        //Grants super access
        public static void GrantSuper()
        {
            if (SuperStatus == false)
            {
                SuperStatus = true;
                WriteLine("SuperStatus access granted!");
                WriteLine("The SuperStatus commands are ADD, REMOVE and DEMO");
            }
            else
            {
                SuperStatus = false;
                WriteLine("SuperStatus access removed!");
            }
        }

        //Adds an item by input
        public static void Add()
        {
            if (!SuperStatus)                                                       //Limited to super access
            {
                WriteLine("You do not have super access!");
                return;
            }

                Target = GetTarget();                                               //Get target inventory
                Item newItem = ItemCreate();                                        //Create the item from input
            if (newItem == null)                                                    //If it's invalid
            {
                WriteLine("Please include an item type, Target inventory and additional data!");
                return;
            }
            GetCurrentInventoryList(Target).Add(newItem);                           //Add it to the inventory
            WriteLine("Item added!");
        }

        //Removes an item by input
        public static Item Remove()
        {
            Item temp = RemoveNoLog(false);                                         //RemoveNoLog is limited by super, input is a bypass that this doesn't get.
            if (temp == null)                                                       //If you failed the no-log, you fail this.
            {
                WriteLine("Item not found!");
                return null;
            }
            WriteLine(temp.Name + " removed!");
            return temp;                                                            //Return the removed item.
        }

        //Lists a helpful output.
        public static void Help()
        {
            WriteLine("  The commands available to you are BUY, SELL, LOOK,");
            WriteLine("  TALK, ME, INTERACT, EXAMINE, RENAME, EQUIP, UNEQUIP,");
            WriteLine("   TAKE, MUTE GO NORTH, GO SOUTH, GO EAST, GO WEST,");
            WriteLine("                   EXIT, HELP, QUIT");
        }

        //Loads and runs a demo version of the game, then reloads this current game.
        public static void Demo()
        {
            if (!SuperStatus)                                   //Limited to super
            {
                WriteLine("You do not have super status!");
                return;
            }

            #region Saving game, loading demo environment
            InternalTesting = true;                             //List that there's internal testing going on

            Save();                                             //Save

            //Note that garbage collection will take everything, since the whole stored heirarchy is gone.
            #region Reinitialize globals (Other than InternalTesting)
            Player = null;
            MainBoard = null;
            Input = "";
            SuperStatus = false;
            CurrentCommand = "";
            Target = null;
            Inventories = new List<Inventory>();
            TextQueue = new Queue<Line>();
            Mute = false;
            MapDraw = true;
            #endregion

            //Reinitalize these
            TextManager.Initialize();
            InventoryManager.TestInitialize();
            EntityManager.TestInitialize();

            WriteLine(" While testing, please ignore requests for user input.");
            WriteLine("");
            Redraw();                                                               //Redraw the fresh map

            if (!ExternalTesting)
            {
                System.Threading.Thread.Sleep(3000);
            }
            Redraw();
            #endregion

            //Note that you have to redraw before each sleep, lest it look wonky during the wait.
            #region Test commands execution
            string testCommand = "";                                                //Test command storage
            foreach (string test in TestCommandList)                                //For each command in the list
            {
                Input = test;                                                       //Input it
                if (Input != "")                                                    //Update command if the testcommand wasn't blank
                    testCommand = ProcessInput(test);                               //Process it
                Redraw();                                                           //Draw a fresh map
                if (!ExternalTesting)
                {
                    System.Threading.Thread.Sleep(400);                             //Give time for user to view
                }

                WriteLine("");
                Commands[testCommand]();                                            //Run it
                WriteLine("");
                ConsoleManager.Redraw();                                            //Redraw
                if (!ExternalTesting)
                    System.Threading.Thread.Sleep(1000);                            //Give time for user to view
            }

            WriteLine("Demo complete! Please wait while your game is reloaded!");

            Redraw();
            if (!ExternalTesting)
                System.Threading.Thread.Sleep(3000);
            #endregion

            InternalTesting = false;                                                //Disable test trigger

            #region Reinitialize globals (Other than InternalTesting)
            Player = null;
            MainBoard = null;
            Input = "";
            SuperStatus = false;
            CurrentCommand = "";
            Target = null;
            Inventories = new List<Inventory>();
            TextQueue = new Queue<Line>();
            Mute = false;
            MapDraw = true;
            #endregion

            //Reload everything from before.
            TextManager.Initialize();
            InventoryManager.Initialize();
            EntityManager.Initialize();
        }

        //Runs a quit dialogue
        public static void Quit()
        {
            WriteLine("Warning! Progress will be lost if you quit without saving! Type \"YES\" to confirm!");
            if (!ExternalTesting)
                if (ReadLine() == "YES")
            {
                StopMusic();
                System.Environment.Exit(0);
            }
        }

        //Notes that there was an invalid command
        public static void InvalidCommand()
        {
            WriteLine("Invalid Command!");
        }
    }
}

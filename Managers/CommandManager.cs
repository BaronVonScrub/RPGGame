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

        public static void Look()
        {
            InventoryView = false;
            Target = GetTarget();
            WriteLine(Target.Description);
        }

        public static void Empty()
        { }

        public static void MuteToggle() => Mute = !Mute;

        public static void Move(MoveCommand direction)
        {
            InventoryView = false;

            List<Entity> goalSquareEntities = MainBoard.GetFromBoard(new Coordinate(Player.position.x + direction.x, Player.position.y + direction.y));
            if (goalSquareEntities != null && goalSquareEntities.Exists(x => x.Passable == false))
            {
                WriteLine("Can't walk there!");
                return;
            }

            if (!CombatCheck(goalSquareEntities))
            {
                return;
            }

            MainBoard.entityPos[Player.position].Remove(Player);
            Player.position.x += direction.x;
            Player.position.y += direction.y;
            MainBoard.AddToBoard(Player);

            CleanUp(MainBoard);
            MonsterGen(direction, Player.DistanceFromCenter());

            foreach (Entity ent in EntityManager.GetLocalEntities(Player, MainBoard).FindAll(x => (x.Name != "Player")))
                WriteLine("You see a " + ent.Name + ent.Status + "\b");

        }

        public static void UnequipByInput() => UnequipFromTargetByName(Player, Input);

        public static void EquipByInput() => EquipToTargetByName(Player, Input);

        public static void Buy()
        {
            Entity other = GetTarget();

            if (Trade(GetTarget(), Player))
                WriteLine("Item bought!");
            else
                WriteLine("Purchase failed!");

            Target = other;
        }

        public static void Sell()
        {
            Entity other = GetTarget();

            if (Trade(Player, GetTarget()))
                WriteLine("Item sold!");
            else
                WriteLine("Sale failed!");

            Target = other;
        }

        public static void ViewInventory(Entity ent)
        {
            Target = ent;
            if (!InventoryIsAccessible(Target))
            {
                WriteLine("Target inventory is not visible.");
                return;
            }

            InventoryView = true;

            GetCurrentInventoryList(Target).Sort(AlphabeticalByName);

            if (!ExternalTesting)
                Console.Clear();


            WriteLine(UNDERLINE + Target.Name.ToUpper() + RESET);
            if (Target == Player)
                Player.StatDisplay();
            GoldDisplay();

            int count = 0;

            foreach (Item item in GetCurrentInventoryList(Target).FindAll(x => (x.GetType().Name != "Gold")))
            {
                count += 1;
                if (!ExternalTesting)
                    if (count % (Console.BufferHeight - 5) == 0)
                    {
                        Console.WriteLine("Press enter for more...");
                        if (InternalTestMode())
                            Console.ReadKey();
                        else
                            System.Threading.Thread.Sleep(3000);
                        Console.Clear();
                    }

                if (item.itemData.ContainsKey("amount"))
                {
                    WriteLine(item.itemData["amount"] + " " + item.Look());
                }
                else
                    WriteLine(item.Look());
            }
            WriteLine(UNDERLINE + "______________________________________________________" + RESET);
            Console.WriteLine("Press enter to continue.");
            if (!ExternalTesting && !InternalTestMode())
                Console.ReadKey();
            else
                System.Threading.Thread.Sleep(3000);
            do
            { TextQueue.Dequeue(); }
            while (TextQueue.Count != 0);
        }

        public static void TradeView()
        {
            ViewInventory(GetTarget());
        }

        public static void LookAtMe()
        {
            ViewInventory(Player);
        }

        public static void Exit()
        {
            if (InventoryView)
                InventoryView = false;
            else
                WriteLine("Only usable when viewing an inventory!");
        }

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

        public static void Examine()
        {
            string data = Strip(Input);
            if (GetCurrentInventoryList(Target) == null)
            {
                WriteLine("Inventory not found!");
                return;
            }

            Item item = GetCurrentInventoryList(Target).Find(x => ((data == x.Name) && (x.Equipped == false)));
            if (item == null)
                item = GetCurrentInventoryList(Target).Find(x => (data == x.Name));

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

            item.Examine();
        }

        public static void Rename()
        {
            Target = GetTarget();
            string data = Strip(Input);
            if (GetCurrentInventoryList(Target) == null)
            {
                WriteLine("Inventory not found!");
                return;
            }
            Item item = GetCurrentInventoryList(Target).Find(x => data.Contains(x.Name));
            if (item != null)
            {
                data = data.Replace(item.Name + " ", "");
                data = Regex.Replace(data, "^[\\s]+|[\\s]+$", "");
                item.Name = data;
                WriteLine("Item renamed!");
            }
            else
                WriteLine("Item not found!");
        }

        public static void Take()
        {
            Target = GetTarget();
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

            Item moveItem = Grab(Target, Player);

            if (moveItem == null)
                return;

            WriteLine(moveItem.Name + " taken!");
            Player.inventory.inventData.Add(moveItem);

            CleanUp(MainBoard);

        }

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

        public static void Add()
        {
            if (SuperStatus)
            {
                Target = GetTarget();
                Item newItem = ItemCreate();
                if (newItem != null)
                {
                    GetCurrentInventoryList(Target).Add(newItem);
                    WriteLine("Item added!");
                }
                else
                    WriteLine("Please include an item type, Target inventory and additional data!");
            }
            else
                WriteLine("You do not have super access!");

        }

        public static Item Remove()
        {
            Item temp = RemoveNoLog(false);
            if (temp == null)
            {
                WriteLine("Item not found!");
                return null;
            }
            WriteLine(temp.Name + " removed!");
            return temp;
        }

        public static void Help()
        {
            WriteLine("  The commands available to you are BUY, SELL, LOOK,");
            WriteLine("  TALK, ME, INTERACT, EXAMINE, RENAME, EQUIP, UNEQUIP,");
            WriteLine("   TAKE, MUTE GO NORTH, GO SOUTH, GO EAST, GO WEST,");
            WriteLine("                   EXIT, HELP, QUIT");
        }

        public static void Demo()
        {
            if (!SuperStatus)
            {
                WriteLine("You do not have super status!");
                return;
            }

            InternalTesting = true;

            Save();

            Player = null;
            MainBoard = null;
            Input = "";
            SuperStatus = false;
            CurrentCommand = "";
            Target = null;
            Inventories = new List<Inventory>();
            TextQueue = new Queue<Line>();
            Mute = false;

            TextManager.Initialize();
            InventoryManager.TestInitialize();
            EntityManager.TestInitialize();

            WriteLine(" While testing, please ignore requests for user input.");
            WriteLine("");
            Redraw();

            string testCommand = "";

            if (!ExternalTesting)
            {
                System.Threading.Thread.Sleep(3000);
            }
            ConsoleManager.Redraw();
            foreach (string test in TestCommandList)
            {
                Input = test;
                WriteLine(Input);
                ConsoleManager.Redraw(); ;
                if (!ExternalTesting)
                {
                    Redraw();
                    System.Threading.Thread.Sleep(400);
                }

                if (Input!="")
                    testCommand = ProcessInput(test);

                WriteLine("");
                Commands[testCommand]();
                WriteLine("");
                ConsoleManager.Redraw();
                if (!ExternalTesting)
                    System.Threading.Thread.Sleep(1000);
            }

            WriteLine("Demo complete! Please wait while your game is reloaded!");

            Redraw();
            System.Threading.Thread.Sleep(3000);

            InternalTesting = false;

            MusicPlayer.Initialize();
            ConsoleManager.Initialize();
            TextManager.Initialize();
            ParseManager.Initialize();
            InventoryManager.Initialize();
            EntityManager.Initialize();
        }

        public static void Quit()
        {
            WriteLine("Warning! Progress will be lost if you quit without saving! Type \"YES\" to confirm!");
            if (ExternalTesting)
            {
                StopMusic();
                System.Environment.Exit(0);
            }
            else
                if (ReadLine() == "YES")
            {
                StopMusic();
                System.Environment.Exit(0);
            }
        }

        public static void InvalidCommand()
        {
            WriteLine("Invalid Command!");
            WriteLine("");
        }
    }
}

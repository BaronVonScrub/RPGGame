using System;
using static RPGGame.GlobalVariables;
using static RPGGame.TextManager;
using static RPGGame.ConsoleManager;
using static RPGGame.ParseManager;
using static RPGGame.ConstantVariables;

namespace RPGGame
{
    class GameLoop
    {
        public static void Run()
        {
            do
            {
                Redraw();

                Input = ReadLine();                                                   
                CurrentCommand = ProcessInput(Input);                                           

                if (CurrentCommand == "")
                {
                    WriteLine("");
                    WriteLine("Invalid Command!");
                    WriteLine("");
                }
                else
                {
                    WriteLine("");
                    Commands[CurrentCommand]();                                                         
                }

            }
            while (CurrentCommand != "^QUIT$" && CurrentCommand != "^TEST$");                                                     
            WriteLine("Gave saved! Press any key to continue...");
            Redraw();
            Console.ReadKey();
        }
    }
}

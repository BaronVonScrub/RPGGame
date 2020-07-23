using System;
using static RPGGame.GlobalVariables;
using static RPGGame.TextTool;
using static RPGGame.ConsoleHelper;
using static RPGGame.ParseTool;
using System.Collections.Generic;
using System.Text;

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
                    WriteLine("");
                }

            }
            while (CurrentCommand != "^QUIT$" && CurrentCommand != "^TEST$");                                                     
            WriteLine("Game saved! Press any key to continue...");
            Redraw();
            Console.ReadKey();
        }
    }
}

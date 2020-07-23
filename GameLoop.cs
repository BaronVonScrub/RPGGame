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

                Input = ReadLine();                                                   //Get console Input
                CurrentCommand = ProcessInput(Input);                                           //Check for valid commands                           //Record current command

                if (CurrentCommand == "")
                {
                    WriteLine("");
                    WriteLine("Invalid Command!");
                    WriteLine("");
                }
                else
                {
                    WriteLine("");
                    Commands[CurrentCommand]();                                                         //Execute current command
                    WriteLine("");
                }

            }
            while (CurrentCommand != "^QUIT$" && CurrentCommand != "^TEST$");                                                     //Continue loop until command is Quit
            WriteLine("Game saved! Press any key to continue...");
            Redraw();
            Console.ReadKey();
        }
    }
}

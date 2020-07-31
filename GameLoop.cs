using System;
using static RPGGame.ConsoleManager;
using static RPGGame.GlobalConstants;
using static RPGGame.GlobalVariables;
using static RPGGame.ParseManager;
using static RPGGame.TextManager;

namespace RPGGame
{
    internal class GameLoop
    {
        public static void Run()
        {
            do
            {
                Redraw();

                Input = ReadLine();
                
                if (Input!="")
                    CurrentCommand = ProcessInput(Input);

                Commands[CurrentCommand]();
                WriteLine("");
            }
            while (true);
        }
    }
}

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
            do                                                  //Repeatedly
            {
                Redraw();                                       //Render the console

                Input = GetInput();                             //Receive the input
                
                if (Input!="")                                  //If the input is blank, don't update command, thus repeating previous command.
                    CurrentCommand = ProcessInput(Input);       //Process the input to parse out the command

                Commands[CurrentCommand]();                     //Execute the command
                WriteLine("");                                  //Add a space
            }
            while (true);                                       //No break condition, as quitting is handled internally.
        }
    }
}

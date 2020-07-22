using System;
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
                ConsoleHelper.Redraw();

                TextTool.input = TextTool.ReadLine();                                                   //Get console TextTool.input
                TextTool.currentCommand = ParseTool.ProcessInput(TextTool.input);                                           //Check for valid commands                           //Record current command

                if (TextTool.currentCommand == "")
                {
                    TextTool.WriteLine("");
                    TextTool.WriteLine("Invalid Command!");
                    TextTool.WriteLine("");
                }
                else
                {
                    TextTool.WriteLine("");
                    ParseTool.commands[TextTool.currentCommand]();                                                         //Execute current command
                    TextTool.WriteLine("");
                }

            }
            while (TextTool.currentCommand != "^QUIT$" && TextTool.currentCommand != "^TEST$");                                                     //Continue loop until command is Quit
            TextTool.WriteLine("Game saved! Press any key to continue...");
            ConsoleHelper.Redraw();
            Console.ReadKey();
        }
    }
}

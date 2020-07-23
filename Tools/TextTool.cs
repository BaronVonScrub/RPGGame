using System;
using System.Collections.Generic;
using static RPGGame.GlobalVariables;
using static RPGGame.InventoryManager;

namespace RPGGame
{
    class TextTool
    {
         public static void Initialize()
        {
            TextQueue = new Queue<Line>();
            WriteLine("                        Welcome!");
            WriteLine("            Type HELP for a list of commands!");
            WriteLine("");
        }

        public static void WriteLine(String inp)                                                          //Outputs a line in green
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(inp);
            Console.ForegroundColor = ConsoleColor.White;
            TextQueue.Enqueue(new Line(inp, ConsoleColor.Green));
            if (TextQueue.Count > 16)
                TextQueue.Dequeue();
        }

        public static String ReadLine()                                                          //Outputs a line in green
        {
            Console.ForegroundColor = ConsoleColor.White;
            String line = Console.ReadLine();
            TextQueue.Enqueue(new Line(line, ConsoleColor.White));
            if (TextQueue.Count > 16)
                TextQueue.Dequeue();
            return line;
        }

        public static void RenderText()
        {
            for (int i = 0; i < TextQueue.Count; i++)
            {
                Line line = TextQueue.Dequeue();
                Console.ForegroundColor = line.col;
                Console.WriteLine(line.lineData);
                TextQueue.Enqueue(line);
            }
        }

        public static void Write(String inp)                                                              //Outputs in green
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(inp);
            Console.ForegroundColor = ConsoleColor.White;
        }
        
        public static void GoldDisplay()
        {
            WriteLine(UNDERLINE + Target.Replace("\\b", "").ToUpper() + RESET);                             //Write inventory title
            WriteLine("GOLD : " + GetGold(Target));                                                         //Write inventory gold 
            WriteLine(UNDERLINE + "______________________________________________________" + RESET);
        }
    }
    struct Line
    {
        public String lineData;
        public ConsoleColor col;

        public Line(String lineData, ConsoleColor col)
        {
            this.lineData = lineData;
            this.col = col;
        }
    }
}

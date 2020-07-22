using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class TextTool
    {
        public static String input { get; set; }                                                                                 //ReadLine storage
        public static String currentCommand { get; set; }                                                                        //Current valid command

        public static Queue<Line> textQueue;

        const string UNDERLINE = "\x1B[4m";                                                                  //The ANSI escape character for underline
        const string RESET = "\x1B[0m";                                                                      //The ANSI escape character to end the underline

        public static void Initialize()
        {
            textQueue = new Queue<Line>();
            WriteLine("                        Welcome!");
            WriteLine("            Type HELP for a list of commands!");
            WriteLine("");
        }

        public static void WriteLine(String inp)                                                          //Outputs a line in green
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(inp);
            Console.ForegroundColor = ConsoleColor.White;
            textQueue.Enqueue(new Line(inp, ConsoleColor.Green));
            if (textQueue.Count > 16)
                textQueue.Dequeue();
        }

        public static String ReadLine()                                                          //Outputs a line in green
        {
            Console.ForegroundColor = ConsoleColor.White;
            String line = Console.ReadLine();
            textQueue.Enqueue(new Line(line, ConsoleColor.White));
            if (textQueue.Count > 16)
                textQueue.Dequeue();
            return line;
        }

        public static void RenderText()
        {
            for (int i = 0; i < textQueue.Count; i++)
            {
                Line line = textQueue.Dequeue();
                Console.ForegroundColor = line.col;
                Console.WriteLine(line.lineData);
                textQueue.Enqueue(line);
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
            WriteLine(UNDERLINE + InventoryManager.target.Replace("\\b", "").ToUpper() + RESET);                             //Write inventory title
            WriteLine("GOLD : " + InventoryManager.GetGold(InventoryManager.target));                                                         //Write inventory gold 
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

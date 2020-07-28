﻿using System;
using static RPGGame.GlobalVariables;
using static RPGGame.ConstantVariables;
using static RPGGame.InventoryManager;

namespace RPGGame
{
    class TextTool
    {
         public static void Initialize()
        {
            WriteLine("                        Welcome!");
            WriteLine("            Type HELP for a list of commands!");
            WriteLine("");
        }

        public static void WriteLine(String inp)                                                          
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(inp);
            Console.ForegroundColor = ConsoleColor.White;
            TextQueue.Enqueue(new Line(inp, ConsoleColor.Green));
            if (TextQueue.Count > 16)
                TextQueue.Dequeue();
        }

        public static string ReadLine()                                                          
        {
            Console.ForegroundColor = ConsoleColor.White;
            string line = Console.ReadLine();
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

        public static void Write(String inp)                                                              
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(inp);
            Console.ForegroundColor = ConsoleColor.White;
        }
        
        public static void GoldDisplay()
        {
            WriteLine(UNDERLINE + Target.inventory.name.ToUpper() + RESET);                             
            WriteLine("GOLD : " + GetGold(Target));                                                         
            WriteLine(UNDERLINE + "______________________________________________________" + RESET);
        }
    }
}

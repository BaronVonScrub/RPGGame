using System;
using static RPGGame.GlobalVariables;
using static RPGGame.ConstantVariables;
using static RPGGame.InventoryManager;
using System.Text.RegularExpressions;
using System.Text;

namespace RPGGame
{
    class TextTool
    {
        public static string AddSpacesBetweenCaps(string inString)
        {
            if (string.IsNullOrWhiteSpace(inString))
                return "";
            StringBuilder outString = new StringBuilder(inString.Length * 2);
            outString.Append(inString[0]);
            for (int i = 1; i < inString.Length; i++)
            {
                if (char.IsUpper(inString[i]) && inString[i - 1] != ' ')
                    outString.Append(' ');
                outString.Append(inString[i]);
            }
            return outString.ToString();
        }

        public static string ToTitleCase(string inp)
        {
            string temp = ToUpperCamelCase(inp);
            temp = AddSpacesBetweenCaps(temp);
            return temp;
        }

        public static string ToUpperCamelCase(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

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
            if (TextQueue.Count > 15)
                TextQueue.Dequeue();
        }

        public static string ReadLine()                                                          
        {
            Console.ForegroundColor = ConsoleColor.White;
            string line = Console.ReadLine();
            TextQueue.Enqueue(new Line(line, ConsoleColor.White));
            if (TextQueue.Count > 15)
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
            WriteLine("GOLD : " + GetGold(Target));                                                         
            WriteLine(UNDERLINE + "______________________________________________________" + RESET);
        }
    }
}

using System;
using System.Text;
using static RPGGame.GlobalConstants;
using static RPGGame.GlobalVariables;
using static RPGGame.InventoryManager;

namespace RPGGame
{
    internal class TextManager
    {
        //This just outputs a welcome message!
        public static void Initialize()
        {
            WriteLine("                        Welcome!");
            WriteLine("            Type HELP for a list of commands!");
            WriteLine("");
        }

        //This adds spaces between the non-leading capitals of a string, converting UpperCamelCase to Title Case 
        public static string AddSpacesBetweenCaps(string inString)
        {
            if (string.IsNullOrWhiteSpace(inString))                            //If it's blank, return blank.
                return "";
            var outString = new StringBuilder(inString.Length * 2);             //New stringbuilder with capacity double the string size (If every letter were capital)
            outString.Append(inString[0]);                                      //Append the first letter of the string
            for (int i = 1; i < inString.Length; i++)                           //For all remaining letters
            {
                if (char.IsUpper(inString[i]) && inString[i - 1] != ' ')        //If the character is upper and it's not preceded by a space
                    outString.Append(' ');                                      //Append a space to the builder
                outString.Append(inString[i]);                                  //Append this character
            }
            return outString.ToString();                                        //Return the output string
        }

        //This converts a camelCase string format to Title Case
        public static string ToTitleCase(string inp)
        {
            string temp = ToUpperCamelCase(inp);            //Convert to upper camel casse
            temp = AddSpacesBetweenCaps(temp);              //Add spaces between the capital letters
            return temp;                                    //Return the string
        }

        //Convert a lowerCamelCase string to UpperCamelCase
        public static string ToUpperCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s))                    //If it's blank, return blank.
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);     //Return the capitalised first letter plus the rest of the string.
        }

        //Custom Writeline function that manually buffers the strings, plus their colour.
        public static void WriteLine(string inp)
        {
            int count = 0;
            string partial = "";
            do
            {
                if (count + Console.WindowWidth < inp.Length)
                    partial = inp.Substring(count, Console.WindowWidth);
                else
                    partial = inp.Substring(count);
                count += Console.WindowWidth;
                TextQueue.Enqueue(new Line(partial, ConsoleColor.Green));
            }
            while (count < inp.Length);
            if (TextQueue.Count > 14)                                   //Limit string buffer to 14
                TextQueue.Dequeue();
        }

        //Custom ReadLine function that manually buffers the strings, plus their colour.
        public static string ReadLine()
        {
            Console.ForegroundColor = ConsoleColor.White;
            string line = "";
            if (!ExternalTesting)
                line = GetInput();                                      //Custom GetInput() function that doesn't append a newline on enter. (Otherwise it makes the map wonky and I
                                                                        //don't want to be forced to redraw it every line and have the game flicker more than is necessary.)
            TextQueue.Enqueue(new Line(line, ConsoleColor.White));
            TextQueue.Enqueue(new Line("", ConsoleColor.White));
            if (TextQueue.Count > 14)                                   //Limit string buffer to 14
                TextQueue.Dequeue();
            return line;
        }

        //Clears the current line of the console.
        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;                  //Stores the current line
            Console.SetCursorPosition(0, Console.CursorTop);            //Positions it at the start of that line
            Console.Write(new string(' ', Console.WindowWidth));        //Overwrites the whole line with space
            Console.SetCursorPosition(0, currentLineCursor);            //Resets the cursor position
        }

        //Rewrites the text in the custom buffer, in the appropriate colour and poisition.
        public static void RenderText()
        {
            for (int i = 0; i < TextQueue.Count; i++)                   //For each line in the queue
            {
                if (TextQueue.Count>15)
                    do
                    {
                        TextQueue.Dequeue();                                //Ensure Textqueue fits
                    }
                    while (TextQueue.Count > 15);

                if (!ExternalTesting)
                    Console.SetCursorPosition(0,14+i);                  //Set the cursor position to the start of the appropriate line
                if (!ExternalTesting)
                    ClearCurrentConsoleLine();
                Line line = TextQueue.Dequeue();                        //Get the stored line
                Console.ForegroundColor = line.col;                     //Set the appropriate colour
                Console.Write(line.lineData);                           //Write the line
                TextQueue.Enqueue(line);                                //Requeue the line for next time
            }
            if (!ExternalTesting)
            {
                Console.SetCursorPosition(0, Console.BufferHeight - 1); //Set the cursor to the bottom
                ClearCurrentConsoleLine();                              //Purge the bottom (Since they don't naturally shift up due to custom GetInput())
            }
        }

        //Custom GetInput command because the regular ReadLine command appends a newline when you hit enter. This caused the map to shift up and
        //off the screen, causing me to have to redraw it and have the game flicker. Creating a custom GetInput so I didn't have to redraw it all the time
        //made sense.
        public static string GetInput()
        {
            Console.ForegroundColor = ConsoleColor.White;
            string inp = "";
            if (ExternalTesting)                                                    //User input is skipped if you're testing.
                return inp;
            ConsoleKey nextChar = ConsoleKey.Backspace;                             //Initalize the stored ConsoleKey.
            do                                                                      //Loop until there's an enter press
            {
                switch (nextChar)                                                   //Switch based on the ConsoleKey
                {
                    case ConsoleKey.Enter:                                          //If it's enter, do nothing. You're going to break the loop anyway.
                        break;
                    case ConsoleKey.Spacebar:                                       //If it's spacebar, append a space to the string.
                        inp += " ";
                        break;
                    case ConsoleKey.Backspace:                                      //If it's backspace
                        if (inp != "")                                              //If the string isn't blank
                            inp = inp.Substring(0, inp.Length - 1);                 //Remove the last character from the stored string
                        Console.Write(" ");                                         //Overwrite the previous character onscreen with a blank.
                        Console.CursorLeft = Math.Max(0, Console.CursorLeft - 1);
                        Console.Write(" ");
                        Console.CursorLeft = Math.Max(0, Console.CursorLeft - 1);   //
                        break;
                    default:                                                        //If it's nothing special, just append it to the string.
                        inp += nextChar.ToString();
                        break;
                }
                nextChar = Console.ReadKey().Key;                                   //Then get a new key!
            }
            while (nextChar != ConsoleKey.Enter);                                   //When there is an Enter press
            Console.ForegroundColor = ConsoleColor.Green;
            return inp;                                                             //Return the stored string
        }

        //Custom write command that writes the given input to screen.
        public static void Write(string inp)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(inp);
            Console.ForegroundColor = ConsoleColor.White;
        }

        //Displays a header for the InventoryView that shows the gold the target has.
        public static void GoldDisplay()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("GOLD : " + GetGold(Target));
            Console.WriteLine(UNDERLINE + "______________________________________________________" + RESET);
        }
    }
}
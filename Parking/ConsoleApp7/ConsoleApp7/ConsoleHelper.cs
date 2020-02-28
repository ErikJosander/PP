using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApp7
{
    public class ConsoleHelper
    {
        /// <summary>
        /// Reads user input from the console.
        /// </summary>
        /// <param name="prompt">The user prompt.</param>
        /// <param name="forceToLowercase">Whether or not to force the user's provided input to lowercase text.</param>
        /// <returns>A user's provided input as a string.</returns>
        public static string ReadInput(string prompt, bool forceToLowercase = false)
        {
            Console.WriteLine();
            Console.Write(prompt);
            string input = Console.ReadLine();
            return forceToLowercase ? input.ToLower() : input;
        }

        /// <summary>
        /// Reads user input from the console.
        /// </summary>
        /// <param name="prompt">The user prompt.</param>
        /// <param name="forceToLowercase">Whether or not to force the user's provided input to lowercase text.</param>
        /// <returns>A user's provided input as a string.</returns>
        public static int ReadIntInput(int range)
        {
            int toReturn = 0;
            bool running = true;
            while (running)
            {
                int input;
                var isInt = int.TryParse(Console.ReadLine(), out input);
                if (!isInt)
                {
                    Console.WriteLine("Error: Not an int");
                    continue;
                }
                if (input < 0 || input > range)
                {
                    Console.WriteLine("Error: out of range");
                    continue;
                }
                if (isInt)
                {
                    toReturn = input;
                    return toReturn;
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Writes the provided message to the console as a line.
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        /// <param name="outputBlankLineBeforeMessage">Whether or not to write a blank line before the message.</param>
        public static void OutputLine(string message, bool outputBlankLineBeforeMessage = true)
        {
            if (outputBlankLineBeforeMessage)
            {
                Console.WriteLine();
            }
            Console.WriteLine(message);
        }

        /// <summary>
        /// Clears the console output.
        /// </summary>
        public static void ClearOutput()
        {
            Console.Clear();
        }


        /// <summary>
        /// Generate a main window for the console
        /// </summary>
        public static void MainWindow()
        {
            Console.WriteLine("(1) to creat vehicle");
            Console.WriteLine("(2) to delete vehicle");
            Console.WriteLine("(3) show all vehicles");
            Console.WriteLine("(4) show history");
            Console.WriteLine("(5) ");
            Console.WriteLine("(6) move vehicle");
            Console.WriteLine("(7) sort vehicles");
            Console.WriteLine("(8) show list");
            Console.WriteLine("(9) between dates");
        }


        /// <summary>
        /// Validates the string input of the regnumber
        /// </summary>
        /// <returns></returns>
        public static bool RegInputValidation(string regnumber)
        {
            List<string> uglywords = new List<string>();
            uglywords.Add("Ugly");

            if (regnumber.Count() > 100)
            {
                return false;
            }
            if (string.IsNullOrEmpty(regnumber))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(regnumber))
            {
                return false;
            }
            if (uglywords.Contains(regnumber))
            {
                return false;
            }
            Regex r = new Regex("^[a-zA-Z0-9--]*$");
            if (!r.IsMatch(regnumber))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Writes the provided message to the console.
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        public static void Output(string message)
        {
            Console.Write(message);
        }

        /// <summary>
        /// Writes a blank line to the console.
        /// </summary>
        public static void OutputBlankLine()
        {
            Console.WriteLine();
        }

    }
}

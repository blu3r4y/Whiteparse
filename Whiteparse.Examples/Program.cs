using System;
using Sprache;
using Whiteparse.Grammar;

namespace Whiteparse.Examples
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine("Whiteparse Grammar Shell");

            while (true)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("> ");
                string line = Console.ReadLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;

                if (line == "exit")
                    break;

                try
                {
                    Specification spec = Specification.FromString(line);
                    Console.WriteLine(spec);
                }
                catch (ParseException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
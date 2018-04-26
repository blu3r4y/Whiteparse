using System;
using System.Text.RegularExpressions;
using Sprache;
using Whiteparse.Grammar;

namespace Whiteparse.Cli
{
    internal static class GrammarShell
    {
        internal static void Repl()
        {
            Console.WriteLine("Whiteparse Grammar Shell");
            Console.WriteLine("- Write 'exit' to leave this shell.");
            Console.WriteLine("- '\\n' will be replaced with a new line character, unless escaped.");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("> ");

                string line = null;
                try
                {
                    line = Console.ReadLine() ?? "";

                    // replace '\n' (but not '\\n') and remove the trailing backslash of escaped control characters
                    line = Regex.Replace(line, @"(?<!\\)\\n", "\n").Replace("\\\\n", "\\n");

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine();
                    Console.WriteLine(line);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;

                if (line != null)
                {
                    if (line == "exit")
                        break;

                    try
                    {
                        Template spec = Template.FromString(line);
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
}
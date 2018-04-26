using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CommandLine;
using Whiteparse.Cli.Options;

namespace Whiteparse.Cli
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            ParserResult<object> result = Parser.Default.ParseArguments<ParseOptions, ShellOptions>(args);
            return result.MapResult(
                (ParseOptions opts) => RunMain(opts),
                (ShellOptions opts) => RunShell(opts),
                errs => 1);
        }

        private static int RunMain(ParseOptions opts)
        {
            string grammar = opts.Grammar ?? File.ReadAllText(opts.GrammarFile);

            int numInputs = opts.InputFiles.Count;
            bool writeToStdOut = opts.OutputFiles.Count == 0;
            bool readFromStdIn = numInputs == 0;

            // create output filenames, if the output is a directory
            IList<string> outputFiles = opts.OutputFiles;
            if (!writeToStdOut)
            {
                string directory = opts.OutputFiles.First();
                if (!File.Exists(directory))
                {
                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                    directory = Path.GetFullPath(directory);

                    if (readFromStdIn)
                    {
                        // decide for some filename, when reading from stdin
                        outputFiles = new List<string> {Path.Combine(directory, "parsed.json")};
                    }
                    else
                    {
                        // map the input files to json output files
                        outputFiles = opts.InputFiles
                            .Select(Path.GetFileNameWithoutExtension)
                            .Select(e => Path.Combine(directory, e + ".json"))
                            .ToList();
                    }
                }
            }

            if (numInputs <= 1)
            {
                // read (from stdin or first input file) and parse
                StreamReader input = readFromStdIn ? new StreamReader(Console.OpenStandardInput()) : new StreamReader(opts.InputFiles.First());
                string json = Whiteparser.ParseJson(grammar, EnumerateStream(input),
                    opts.InvariantCulture ? CultureInfo.InvariantCulture : null, true);

                // write
                if (writeToStdOut) Console.Write(json);
                else File.WriteAllText(outputFiles.First(), json);
            }

            else
            {
                // process more than one input file 
                for (var i = 0; i < numInputs; i++)
                {
                    // read, parse, write
                    var input = new StreamReader(opts.InputFiles[i]);
                    string json = Whiteparser.ParseJson(grammar, EnumerateStream(input),
                        opts.InvariantCulture ? CultureInfo.InvariantCulture : null, true);
                    File.WriteAllText(outputFiles[i], json);
                }
            }

            return 0;
        }

        private static int RunShell(ShellOptions opts)
        {
            GrammarShell.Repl();
            return 0;
        }

        /* helper */

        private static IEnumerable<char> EnumerateStream(TextReader stream)
        {
            for (int i = stream.Read(); i != -1; i = stream.Read())
                yield return (char) i;
        }
    }
}
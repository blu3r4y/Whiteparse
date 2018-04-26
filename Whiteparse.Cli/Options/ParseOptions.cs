using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace Whiteparse.Cli.Options
{
    [Verb("parse", HelpText = "Parse input based on a Whiteparse grammar template.")]
    internal class ParseOptions
    {
        [Option('g', "grammar", SetName = "grammar-string", Required = true,
            HelpText = "A Whiteparse grammar template.", MetaValue = "GRAMMAR")]
        public string Grammar => _grammar;

        [Option('f', "grammar-file", SetName = "grammar-file", Required = true,
            HelpText = "Read grammar from a file instead of the argument string.", MetaValue = "grammar.whp")]
        public string GrammarFile => _grammarFile;

        [Option('i', "input",
            HelpText = "Input files to be parsed. Read from STDIN, if not specified.", MetaValue = "input.in")]
        public IList<string> InputFiles => _inputFiles;

        [Option('o', "output",
            HelpText = "Output filenames to store parsed json files or a single output directory, where files are automatically stored. " +
                       "Write to STDOUT, if not specified.", MetaValue = "output.json")]
        public IList<string> OutputFiles => _outputFiles;

        [Option("invariant-culture", HelpText = "Use the invariant culture for parsing.", Default = false)]
        public bool InvariantCulture => _invariantCulture;

        private readonly string _grammar;
        private readonly string _grammarFile;
        private readonly IList<string> _inputFiles;
        private readonly IList<string> _outputFiles;
        private readonly bool _invariantCulture;

        [Usage(ApplicationAlias = "whiteparse")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Parse an input file",
                    new ParseOptions("$a $b $c", null, new[] {"input.in"}));
                yield return new Example("Read the grammar from a file",
                    new ParseOptions(null, "grammar.whp", new[] {"input.in"}));
                yield return new Example("Write the output to a file",
                    new ParseOptions(null, "grammar.whp", new[] {"input.in"}, new[] {"output.json"}));
                yield return new Example("Parse multiple input files",
                    new ParseOptions(null, "grammar.whp", new[] {"input1.in", "input2.in"}, new[] {"output1.json", "output2.json"}));
                yield return new Example("Parse multiple input files to a directory",
                    new ParseOptions(null, "grammar.whp", new[] {"input1.in", "input2.in", "input3.in"}, new[] {"/path/to/output"}));
            }
        }

        public ParseOptions(string grammar, string grammarFile,
            IEnumerable<string> inputFiles = null, IEnumerable<string> outputFiles = null,
            bool invariantCulture = false)
        {
            IList<string> inputFilesList = inputFiles?.ToList() ?? new List<string>();
            IList<string> outputFilesList = outputFiles?.ToList() ?? new List<string>();

            // infer that '-' as an output filename implies stdout
            if (outputFilesList.Count() == 1 && outputFilesList.First() == "-")
                outputFiles = new string[0];

            int numInputs = inputFilesList.Count();
            int numOutputs = outputFilesList.Count();

            // the number of inputs must match the number of outputs (or a output directory)
            if (numInputs > 0 && numOutputs > 1 && numInputs != numOutputs)
            {
                throw new InvalidOperationException("You need to specify as many output files as input files or a single output directory.");
            }

            // set values
            _grammar = grammar;
            _grammarFile = grammarFile;
            _inputFiles = inputFilesList;
            _outputFiles = outputFilesList;
            _invariantCulture = invariantCulture;
        }
    }
}
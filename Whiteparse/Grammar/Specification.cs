using System.Collections.Generic;
using System.Linq;
using Whiteparse.Grammar.Tokens;

namespace Whiteparse.Grammar
{
    public class Specification
    {
        public IEnumerable<Token> Tokens { get; }
        public IEnumerable<Variable> Variables { get; }

        public Specification(IEnumerable<Token> tokens = null, IEnumerable<Variable> variables = null)
        {
            Tokens = tokens ?? new List<Token>();
            Variables = variables ?? new List<Variable>();
        }

        public static Specification FromString(string input)
        {
            return Parser.Parse(input);
        }

        public override string ToString()
        {
            return "Tokens:" + Tokens.Select(t => t.ToString()).ToIndentedBlock() + "\n"
                   + "Variables:" + Variables.Select(t => t.ToString()).ToIndentedBlock();
        }
    }
}
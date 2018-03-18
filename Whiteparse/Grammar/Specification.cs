using System.Collections.Generic;
using System.Linq;
using Whiteparse.Grammar.Tokens;

namespace Whiteparse.Grammar
{
    /// <summary>
    /// Represents a Whiteparse grammar
    /// </summary>
    public class Specification
    {
        /// <summary>
        /// The list of tokens which make up the Whiteparse grammar
        /// </summary>
        public IEnumerable<Token> Tokens { get; }

        /// <summary>
        /// Used variables within the Whiteparse grammar
        /// </summary>
        public IEnumerable<Variable> Variables { get; }

        /// <summary>
        /// Create a new Whiteparse grammer by a list of tokens and variables
        /// </summary>
        /// <param name="tokens">A list of tokens to be parsed</param>
        /// <param name="variables">A list of variables (optional)</param>
        public Specification(IEnumerable<Token> tokens = null, IEnumerable<Variable> variables = null)
        {
            Tokens = tokens ?? new List<Token>();
            Variables = variables ?? new List<Variable>();
        }

        /// <summary>
        /// Parse the grammar specification from text input to a <see cref="Specification"/> object
        /// </summary>
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
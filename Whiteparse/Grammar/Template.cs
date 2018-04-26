using System.Collections.Generic;
using System.Linq;
using Whiteparse.Grammar.Semantics;
using Whiteparse.Grammar.Tokens;

namespace Whiteparse.Grammar
{
    /// <summary>
    /// Represents a Whiteparse grammar, parsed by the <see cref="TemplateParser"/>
    /// </summary>
    public class Template
    {
        /// <summary>
        /// The list of tokens which make up the Whiteparse grammar
        /// </summary>
        public IList<Token> Tokens { get; }

        /// <summary>
        /// Used variables within the Whiteparse grammar
        /// </summary>
        public ISet<Variable> Variables { get; }
        
        /// <summary>
        /// Holds the semantic scope tree
        /// </summary>
        internal Scope GlobalScope { get; }

        /// <summary>
        /// Create a new Whiteparse grammer by a list of tokens and variables.
        /// The constructor will also test the semantics of this <see cref="Template"/>
        /// and only create the object on a positive result.
        /// </summary>
        /// <param name="tokens">A list of tokens to be parsed</param>
        /// <param name="variables">A list of variables (optional)</param>
        /// <exception cref="GrammarException">If an error occurs during semantic resolution</exception>
        public Template(IList<Token> tokens = null, IEnumerable<Variable> variables = null)
        {
            Tokens = tokens ?? new List<Token>();
            Variables = new HashSet<Variable>(variables ?? new HashSet<Variable>());
            
            GlobalScope = Resolver.Resolve(Tokens, Variables);
        }

        /// <summary>
        /// Parse the grammar template from text input to a <see cref="Template"/> object
        /// </summary>
        public static Template FromString(string input)
        {
            return TemplateParser.Parse(input);
        }

        public override string ToString()
        {
            return "Tokens:" + Tokens.Select(t => t.ToString()).ToIndentedBlock() + "\n"
                   + "Variables:" + Variables.Select(t => t.ToString()).ToIndentedBlock();
        }
    }
}
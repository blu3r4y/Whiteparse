using System.Collections.Generic;
using System.Linq;
using Whiteparse.Grammar.Tokens;

namespace Whiteparse.Grammar.Semantics
{
    /// <summary>
    /// Performs both a static analysis of the grammar and some semantic resolutions  
    /// </summary>
    internal static class Resolver
    {
        /// <summary>
        /// Check semantic rules in the token and variable definitions and return a scope tree
        /// </summary>
        /// <param name="tokens">A list of tokens to be parsed</param>
        /// <param name="variables">A list of variables</param>
        /// <returns>The scope tree, which can be used for traversing the grammar</returns>
        /// <exception cref="GrammarException">On semantic errors</exception>
        internal static Scope Resolve(IList<Token> tokens, ISet<Variable> variables)
        {
            // check for named token duplicates in global scope
            List<string> duplicateNamedTokens = GetDuplicateNamedTokens(tokens);
            if (duplicateNamedTokens.Any())
            {
                throw new GrammarException("Semantic error: Duplicate named tokens detected in the global scope: '" +
                                           string.Join("', '", duplicateNamedTokens) + "'.");
            }

            // check for variable name duplicates
            List<string> duplicateVariables = GetDuplicateVariableNames(variables);
            if (duplicateVariables.Any())
            {
                throw new GrammarException("Semantic error: Duplicate variable definitions detected: '" +
                                           string.Join("', '", duplicateVariables) + "'.");
            }

            // check for variables without tokens
            foreach (Variable variable in variables)
            {
                if (!variable.InnerTokens.Any())
                    throw new GrammarException($"Semantic error: Variable '{variable.Name}' doesn't hold any tokens.");
            }

            // store a variable reference in all tokens which are named like the variable
            IEnumerable<NamedToken> allNamedTokens = ExtractAllNamedTokens(tokens.Concat(variables.SelectMany(e => e.InnerTokens))).ToList();
            foreach (NamedToken namedToken in allNamedTokens)
            {
                namedToken.ReferencedVariable = variables.FirstOrDefault(e => e.Name == namedToken.Name);
            }

            // build a scope tree (which checks some semantics during the build process)
            var scope = new Scope(tokens);

            return scope;
        }

        private static List<string> GetDuplicateNamedTokens(IEnumerable<Token> tokens)
        {
            return tokens.OfType<NamedToken>().Select(t => t.Name)
                .SelectDuplicates().ToList();
        }

        private static List<string> GetDuplicateVariableNames(IEnumerable<Variable> variables)
        {
            return variables.Select(v => v.Name)
                .SelectDuplicates().ToList();
        }

        private static IEnumerable<NamedToken> ExtractAllNamedTokens(IEnumerable<Token> tokens)
        {
            return tokens
                .Where(e => e is NamedToken || e is ITokenContainer)
                .SelectMany(e => e is NamedToken ? Enumerable.Repeat(e, 1) : ExtractAllNamedTokens((e as ITokenContainer)?.InnerTokens))
                .OfType<NamedToken>();
        }
    }
}
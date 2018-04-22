using System.Collections.Generic;
using Whiteparse.Grammar.Tokens;
using Whiteparse.Grammar.Tokens.Ranges;

namespace Whiteparse.Grammar.Semantics
{
    /// <summary>
    /// Represents a scope tree, which structures the grammar into nodes and scoped areas
    /// </summary>
    public class Scope : List<Scope.Node>
    {
        /// <summary>
        /// Holds a <see cref="Token"/> and a possible inner <see cref="Scope"/>
        /// </summary>
        public class Node
        {
            public Token Token { get; }
            public Scope InnerScope { get; }

            public Node(Token token, Scope innerScope = null)
            {
                Token = token;
                InnerScope = innerScope;
            }
        }

        /// <summary>
        /// The outer enclosing <see cref="Scope"/> (or null if not specified)
        /// </summary>
        public Scope Outer { get; }

        /// <summary>
        /// A global scope has no outer scope
        /// </summary>
        public bool IsGlobal => Outer == null;

        private readonly Dictionary<string, NamedToken> _namedTokens = new Dictionary<string, NamedToken>();

        /// <summary>
        /// Initializes the scope tree, based on a pre-processed collection of tokens
        /// </summary>
        /// <param name="tokens">A list of tokens</param>
        /// <param name="outer">An optional reference to the outer scope</param>
        /// <exception cref="GrammarException">On semantic errors</exception>
        public Scope(IEnumerable<Token> tokens, Scope outer = null)
        {
            Outer = outer;
            BuildScopeTree(tokens);
        }

        public NamedToken FindNamedToken(string name)
        {
            return _namedTokens.TryGetValue(name, out NamedToken token) ? token : Outer?.FindNamedToken(name);
        }

        private void BuildScopeTree(IEnumerable<Token> tokens)
        {
            foreach (Token token in tokens)
            {
                // extract inner tokens
                IEnumerable<Token> innerTokens = null;
                switch (token)
                {
                    case ITokenContainer container:
                        innerTokens = container.InnerTokens;

                        // check if token ranges within list tokens are reachable
                        if (container is ListToken listToken && listToken.Range is TokenRange tokenRange && FindNamedToken(tokenRange.ReferencedName) == null)
                            throw new GrammarException($"Semantic error: The referenced token '{tokenRange.ReferencedName}' in the list token is not defined.");
                        break;

                    case NamedToken named:
                        if (named.HasReferencedVariable)
                        {
                            innerTokens = named.ReferencedVariable.InnerTokens;

                            // tokens within variables may not use their own name again
                            if (FindNamedToken(named.Name) != null)
                                throw new GrammarException($"Semantic error: Found recursive definition in variable '{named.Name}'.");
                        }

                        AddNamedToken(named);
                        break;
                }

                if (innerTokens != null)
                {
                    // create a new scope for the inner tokens
                    var innerScope = new Scope(innerTokens, this);
                    Add(new Node(token, innerScope));
                }
                else
                {
                    // simple node within the current scope
                    Add(new Node(token));
                }
            }
        }

        private void AddNamedToken(NamedToken token)
        {
            // only search the current scope to allow variable shadowing
            if (_namedTokens.ContainsKey(token.Name))
                throw new GrammarException($"Semantic error: The named token '{token.Name}' was declared multiple times.");

            _namedTokens.Add(token.Name, token);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Whiteparse.Grammar.Tokens
{
    /// <summary>
    /// Data type of a token
    /// </summary>
    public enum TokenType
    {
        Auto,
        Int,
        Bool,
        Double,
        String,
    }

    /// <summary>
    /// A token which parses the value in the input and references it with a specified <see cref="Name"/>
    /// </summary>
    public class NamedToken : Token
    {
        /// <summary>
        /// The name of the token in the resulting object
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The structured name, if specified (or null if not specified)
        /// </summary>
        public string[] StructuredName { get; }

        /// <summary>
        /// The referenced variable (or null if not specified)
        /// </summary>
        public Variable ReferencedVariable { get; internal set; }

        /// <summary>
        /// The data type of this token
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// If this <see cref="NamedToken"/> is specified by a structured name
        /// </summary>
        public bool HasStructuredName => StructuredName != null;
        
        /// <summary>
        /// If this <see cref="NamedToken"/> references a <see cref="Variable"/>
        /// </summary>
        public bool HasReferencedVariable => ReferencedVariable != null;

        /// <summary>
        /// Create a <see cref="NamedToken"/>
        /// </summary>
        /// <param name="name">The token name</param>
        /// <param name="type">Force the data type of this token during parsing</param>
        /// <param name="hidden">Hidden tokens are hidden in the final object</param>
        /// <exception cref="ArgumentException">If the token name contains invalid characters</exception>
        /// <exception cref="ArgumentNullException">If the token name is null</exception>
        public NamedToken(string name, TokenType type = TokenType.Auto, bool hidden = false) : base(hidden)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (name.Contains('.'))
                throw new ArgumentException($"Parameter {nameof(name)} must not contain dots '.'");

            Name = name;
            Type = type;
            StructuredName = null;
        }

        /// <summary>
        /// Create a structured <see cref="NamedToken"/>, consisting of two or more hierarchic names.
        /// Structured tokens implicitly create a object structure.
        /// If only a single element is provided, this cosntructor behaves the same like the simple constructor.
        /// </summary>
        /// <param name="structuredName">A collection of two or more names</param>
        /// <param name="type">Force the data type of this token during parsing</param>
        /// <param name="hidden">Hidden tokens are hidden in the final object</param>
        /// <exception cref="ArgumentException">If any of the elements within the collections contains invalid characters</exception>
        /// <exception cref="ArgumentNullException">If the structured name collection is null</exception>
        public NamedToken(ICollection<string> structuredName, TokenType type = TokenType.Auto, bool hidden = false) : base(hidden)
        {
            if (structuredName == null || structuredName.Count == 0)
                throw new ArgumentNullException(nameof(structuredName));
            if (structuredName.Any(n => n.Contains('.')))
                throw new ArgumentException($"Elements of {nameof(structuredName)} must not contain dots '.'");

            if (structuredName.Count > 1)
            {
                // the name field still encodes the hierachy, separated by dots
                Name = string.Join(".", structuredName);
                StructuredName = structuredName.ToArray();
            }
            else
            {
                // fallback to simple constructor
                Name = structuredName.First();
                StructuredName = null;
            }
            
            Type = type;
        }

        /// <summary>
        /// Create a <see cref="NamedToken"/> with a referenced <see cref="Variable"/>.
        /// The name is implied by the variable name.
        /// </summary>
        /// <param name="variable">The variable</param>
        /// <param name="type">Force the data type of this token during parsing</param>
        /// <param name="hidden">Hidden tokens are hidden in the final object</param>
        /// <exception cref="ArgumentNullException">If the variable is null</exception>
        public NamedToken(Variable variable, TokenType type = TokenType.Auto, bool hidden = false) : base(hidden)
        {
            ReferencedVariable = variable ?? throw new ArgumentNullException(nameof(variable));
            Name = variable.Name;
            Type = type;
            StructuredName = null;
        }

        public override string ToString()
        {
            string type = Type != TokenType.Auto ? $", {Type}" : "";
            string hidden = Hidden ? ", Hidden" : "";
            string structered = HasStructuredName ? ", Structured" : "";
            string variable = HasReferencedVariable ? ", ReferencesVariable" : "";
            return $"NamedToken<\"{Name}\"{type}{structered}{variable}{hidden}>";
        }
    }
}
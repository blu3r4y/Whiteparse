using System;
using System.Collections.Generic;
using System.Linq;

namespace Whiteparse.Grammar.Tokens
{
    public enum TokenDataType
    {
        Auto,
        Int,
        Bool,
        Float,
        String,
    }

    public class NamedToken : Token
    {
        public string Name { get; }
        public string[] StructuredName { get; }
        public TokenDataType Type { get; }

        public bool HasStructuredNames => StructuredName != null;

        /// <summary>
        /// Create a named token
        /// </summary>
        /// <param name="name">The token name</param>
        /// <param name="type">Force the data type of this token during parsing</param>
        /// <param name="hidden">Hidden tokens are hidden in the final object</param>
        /// <exception cref="ArgumentException">If the token name contains invalid characters</exception>
        /// <exception cref="ArgumentNullException">If the token name is null</exception>
        public NamedToken(string name, TokenDataType type = TokenDataType.Auto, bool hidden = false) : base(hidden)
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
        /// Create a structured named token, consisting of two or more hierarchic names.
        /// Structured tokens implicitly create a object structure.
        /// If only a single element is provided, this cosntructor behaves the same like the simple constructor.
        /// </summary>
        /// <param name="structuredName">A collection of two or more named</param>
        /// <param name="type">Force the data type of this token during parsing</param>
        /// <param name="hidden">Hidden tokens are hidden in the final object</param>
        /// <exception cref="ArgumentException">If any of the elements within the collections contains invalid characters</exception>
        /// <exception cref="ArgumentNullException">If the structured name collection is null</exception>
        public NamedToken(ICollection<string> structuredName, TokenDataType type = TokenDataType.Auto, bool hidden = false) : base(hidden)
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

        public override string ToString()
        {
            string type = Type != TokenDataType.Auto ? $", {Type}" : "";
            string hidden = Hidden ? ", Hidden" : "";
            string structered = HasStructuredNames ? ", Structured" : "";
            return $"NamedToken<\"{Name}\"{type}{structered}{hidden}>";
        }
    }
}
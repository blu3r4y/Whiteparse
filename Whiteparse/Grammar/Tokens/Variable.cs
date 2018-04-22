using System;
using System.Collections.Generic;
using System.Linq;

namespace Whiteparse.Grammar.Tokens
{
    /// <summary>
    /// A variable creates a new field in the final object and can be used to structure the final object.
    /// </summary>
    public class Variable : ITokenContainer
    {
        /// <summary>
        /// The name of the variable (which needs to reference a <see cref="NamedToken"/>)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// If specified, the variable will be parsed to a runtime object type using reflection (c# only)
        /// </summary>
        public string ObjectType { get; }

        /// <summary>
        /// A variable is defined by a list of inner tokens
        /// </summary>
        public IEnumerable<Token> InnerTokens { get; }

        /// <summary>
        /// Creates a new <see cref="Variable"/>
        /// </summary>
        /// <param name="name">The name of the variables (which needs to reference a <see cref="NamedToken"/>)</param>
        /// <param name="innerTokens">The inner token specifying this variable</param>
        /// <param name="objectType">The runtime target type this variable should be parsed to (c# only)</param>
        /// <exception cref="ArgumentNullException">If the name or definition is null</exception>
        public Variable(string name, IEnumerable<Token> innerTokens, string objectType = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            InnerTokens = innerTokens ?? throw new ArgumentNullException(nameof(innerTokens));
            ObjectType = objectType;
        }

        public override string ToString()
        {
            string typed = ObjectType != null ? $", {ObjectType}" : "";
            return $"Variable<\"{Name}\"{typed}> " + InnerTokens.Select(t => t.ToString()).ToIndentedBlock();
        }
    }
}
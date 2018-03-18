using System;
using System.Collections.Generic;
using System.Linq;

namespace Whiteparse.Grammar.Tokens
{
    /// <summary>
    /// A variable creates a new field in the final object and can be used to structure the final object.
    /// </summary>
    public class Variable
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
        public IEnumerable<Token> Definition { get; }

        /// <summary>
        /// Creates a new <see cref="Variable"/>
        /// </summary>
        /// <param name="name">The name of the variables (which needs to reference a <see cref="NamedToken"/>)</param>
        /// <param name="definition">The inner token specifying this variable</param>
        /// <param name="objectType">The runtime target type this variable should be parsed to (c# only)</param>
        /// <exception cref="ArgumentNullException">If the name or definition is null</exception>
        public Variable(string name, IEnumerable<Token> definition, string objectType = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            ObjectType = objectType;
        }

        public override string ToString()
        {
            string typed = ObjectType != null ? $", {ObjectType}" : "";
            return $"Variable<\"{Name}\"{typed}> " + Definition.Select(t => t.ToString()).ToIndentedBlock();
        }
    }
}
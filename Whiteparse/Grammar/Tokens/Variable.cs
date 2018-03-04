using System;
using System.Collections.Generic;
using System.Linq;

namespace Whiteparse.Grammar.Tokens
{
    public class Variable
    {
        public string Name { get; }
        public string ObjectType { get; }
        public IEnumerable<Token> Definition { get; }

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
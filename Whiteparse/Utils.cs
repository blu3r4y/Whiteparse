using System;
using System.Collections.Generic;
using System.Linq;

namespace Whiteparse
{
    public static class Utils
    {
        public static string ToIndentedBlock(this IEnumerable<string> elements, int level = 1)
        {
            var spaces = new string(' ', level * 3);
            return "\n" +
                   string.Join("\n", elements
                       .SelectMany(s => s.Split(new[] {'\n'}, StringSplitOptions.None))
                       .Select(s => spaces + s));
        }
    }
}
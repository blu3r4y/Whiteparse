using System;
using System.Collections.Generic;
using System.Linq;

namespace Whiteparse.Grammar
{
    internal static class Utils
    {
        internal static string ToIndentedBlock(this IEnumerable<string> elements, int level = 1)
        {
            var spaces = new string(' ', level * 3);
            return "\n" +
                   string.Join("\n", elements
                       .SelectMany(s => s.Split(new[] {'\n'}, StringSplitOptions.None))
                       .Select(s => spaces + s));
        }

        /// <summary>
        /// Extract a list which only contains entries, that occur one or more times in the enumerable
        /// </summary>
        /// <param name="enumerable">The enumerable which may contains duplicates</param>
        /// <typeparam name="T">The element type to be compared</typeparam>
        /// <returns>A list holding only duplicate entries</returns>
        internal static IEnumerable<T> SelectDuplicates<T>(this IEnumerable<T> enumerable)
        {
            return enumerable
                .GroupBy(x => x)
                .Where(c => c.Count() > 1)
                .Select(e => e.Key);
        }
    }
}
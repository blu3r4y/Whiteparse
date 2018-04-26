using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using Newtonsoft.Json;
using Whiteparse.Grammar;

namespace Whiteparse
{
    /// <summary>
    /// A data parser and reverse templating engine for parsing whitespace-delimited text
    /// </summary>
    public class Whiteparser
    {
        /// <summary>
        /// The grammar which is used to parse input 
        /// </summary>
        public Template Template { get; }

        public CultureInfo Culture { get; }

        /// <summary>
        /// Initialize the <see cref="Whiteparser"/> based on a grammar
        /// </summary>
        /// <param name="template">The grammar which is used to parse input </param>
        /// <param name="culture">The culture for numeric parsing</param>
        public Whiteparser(Template template, CultureInfo culture = null)
        {
            Template = template;
            Culture = culture ?? CultureInfo.CurrentCulture;
        }

        /* the parser */

        /// <summary>
        /// Parse the input text to an anonymous object
        /// </summary>
        /// <param name="input">The input text to parse</param>
        /// <returns>An anonymous object (use the dynamic datatype to access fields during runtime)</returns>
        public object ParseObject(IEnumerable<char> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var interpreter = new ScopeInterpreter(input, Template.GlobalScope, Culture);
            Dictionary<string, object> result = interpreter.ParseInput();
            ExpandoObject expando = DictionaryToExpando(result);

            return expando;
        }

        /// <summary>
        /// Parse the input text to a json string
        /// </summary>
        /// <param name="input">The input text to parse</param>
        /// <param name="indented">Properly indent the resulting string</param>
        /// <returns>A json string</returns>
        public string ParseJson(IEnumerable<char> input, bool indented = false)
        {
            var result = ParseObject(input);
            return JsonConvert.SerializeObject(result, indented ? Formatting.Indented : Formatting.None);
        }
        
        /* some convenience functions for one-line-parsing */

        public static Whiteparser FromTemplate(Template template, CultureInfo culture = null)
        {
            return new Whiteparser(template, culture);
        }

        public static Whiteparser FromTemplate(string template, CultureInfo culture = null)
        {
            return FromTemplate(Template.FromString(template), culture);
        }

        public static object ParseObject(Template template, IEnumerable<char> input, CultureInfo culture = null)
        {
            return FromTemplate(template, culture).ParseObject(input);
        }

        public static object ParseObject(string template, IEnumerable<char> input, CultureInfo culture = null)
        {
            return FromTemplate(template, culture).ParseObject(input);
        }

        public static string ParseJson(Template template, IEnumerable<char> input, CultureInfo culture = null)
        {
            return FromTemplate(template, culture).ParseJson(input);
        }

        public static string ParseJson(string template, IEnumerable<char> input, CultureInfo culture = null)
        {
            return FromTemplate(template, culture).ParseJson(input);
        }

        /* helpers */

        private static ExpandoObject DictionaryToExpando(IDictionary<string, object> input)
        {
            var expando = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>) expando;

            foreach (KeyValuePair<string, object> pair in input)
            {
                switch (pair.Value)
                {
                    // handle nested dictionaries
                    case IDictionary<string, object> nestedDict:
                        expandoDict.Add(pair.Key, DictionaryToExpando(nestedDict));
                        break;

                    // handle nested lists
                    case ICollection<object> nestedList:
                        var newNestedList = new List<object>();
                        foreach (object nestedListItem in nestedList)
                        {
                            // convert dictiony elements in nested lists
                            if (nestedListItem is IDictionary<string, object> nestedListDictElement)
                                newNestedList.Add(DictionaryToExpando(nestedListDictElement));
                            else
                                newNestedList.Add(nestedListItem);
                        }

                        expandoDict.Add(pair.Key, newNestedList);
                        break;

                    // handle all other (primitive) types
                    default:
                        expandoDict.Add(pair);
                        break;
                }
            }

            return expando;
        }
    }
}
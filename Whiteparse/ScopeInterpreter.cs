using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Whiteparse.Grammar.Semantics;
using Whiteparse.Grammar.Tokens;
using Whiteparse.Grammar.Tokens.Ranges;

namespace Whiteparse
{
    internal class ScopeInterpreter
    {
        private readonly Scope _global;
        private readonly CultureInfo _culture;
        private readonly IEnumerator<char> _text;
        private readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();
        private bool _eof = false;

        internal ScopeInterpreter(IEnumerable<char> input, Scope scope, CultureInfo culture)
        {
            _text = input.GetEnumerator();
            _global = scope;
            _culture = culture;

            // set enumerator to first character
            if (!_text.MoveNext())
                _eof = true;
        }

        internal Dictionary<string, object> ParseInput()
        {
            return ParseScope(_global);
        }

        /// <summary>
        /// Scan zero or more leading whitespace characters, followed by one or more non-whitespace characters.
        /// The scanned non-whitespace characters will be returned. 
        /// </summary>
        private string NextInputToken()
        {
            // end reached
            if (_eof)
                return null;

            var builder = new StringBuilder();

            // skip leading whitespace
            while (char.IsWhiteSpace(_text.Current))
            {
                if (!_text.MoveNext())
                {
                    _eof = true;
                    throw new ParserException($"Unexpected end of file.");
                }
            }

            // parse token
            while (!char.IsWhiteSpace(_text.Current))
            {
                builder.Append(_text.Current);
                if (!_text.MoveNext())
                {
                    _eof = true;
                    break;
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Scan whitespace until we encounter the first '\n' character
        /// </summary>
        private void NextNewLineToken()
        {
            // end reached
            if (_eof)
                throw new ParserException($"Unexpected end of file.");

            while (_text.Current != '\n')
            {
                // move on
                if (!_text.MoveNext())
                {
                    _eof = true;
                    throw new ParserException($"Unexpected end of file.");
                }
            }
        }

        private Dictionary<string, object> ParseScope(Scope scope,
            ImmutableDictionary<Scope, Dictionary<string, object>> symbols = null)
        {
            if (symbols == null)
                symbols = ImmutableDictionary.CreateBuilder<Scope, Dictionary<string, object>>().ToImmutable();

            var dict = new Dictionary<string, object>();
            symbols = symbols.Add(scope, dict);

            // parse tokens in this scope
            foreach (Scope.Node node in scope)
            {
                Token token = node.Token;
                if (node.HasInnerScope)
                {
                    switch (token)
                    {
                        case ListToken list:
                            ParseListToken(list, node, scope, symbols, dict);
                            break;
                        case InlineListToken _:
                            ParseInlineListToken(node, symbols, dict);
                            break;
                        case NamedToken named when named.HasReferencedVariable:
                            ParseVariableNameToken(named, node, symbols, dict);
                            break;
                        default:
                            throw new ParserException("Invalid token encountered during scope iteration " +
                                                      "on a node with an inner scope:\n{token}");
                    }
                }
                else
                {
                    switch (token)
                    {
                        case NamedToken named when !named.HasReferencedVariable:
                            ParseNamedToken(named, dict);
                            break;
                        case LiteralToken literal:
                            ParseLiteralToken(literal);
                            break;
                        case RegExToken regex:
                            // TODO: do not ignore the regex result
                            ParseRegExToken(regex, out _);
                            break;
                        case NewLineToken _:
                            NextNewLineToken();
                            break;
                        default:
                            throw new ParserException("Invalid token encountered during scope iteration:\n{{token}}");
                    }
                }
            }

            // remove hidden tokens once they are not needed anymore
            foreach (NamedToken named in scope.Select(n => n.Token).OfType<NamedToken>().Where(t => t.Hidden))
            {
                dict.Remove(named.Name);
            }

            return dict;
        }

        private void ParseVariableNameToken(NamedToken namedToken, Scope.Node node,
            ImmutableDictionary<Scope, Dictionary<string, object>> symbolTable, IDictionary<string, object> dict)
        {
            Dictionary<string, object> innerDict = ParseScope(node.InnerScope, symbolTable);

            // if the variable is specified by only one token, we treat this as
            // a "renaming" action and surpress the additional inner wrapper
            dict.Add(namedToken.Name, innerDict.Count == 1 ? innerDict.Values.First() : innerDict);
        }

        private void ParseInlineListToken(Scope.Node node,
            ImmutableDictionary<Scope, Dictionary<string, object>> symbolTable, IDictionary<string, object> dict)
        {
            Dictionary<string, object> innerDict = ParseScope(node.InnerScope, symbolTable);
            string inlineListName = string.Concat(innerDict.Keys.Select(e => e.ToLower()));
            dict.Add(inlineListName, innerDict.Values);
        }

        private void ParseListToken(ListToken listToken, Scope.Node node, Scope scope,
            ImmutableDictionary<Scope, Dictionary<string, object>> symbolTable, IDictionary<string, object> dict)
        {
            if (listToken.Delimiters != null)
                throw new NotImplementedException("List delimiters are not supported yet.");

            // get the number of iterations needed
            var repetitions = 0;
            switch (listToken.Range)
            {
                // automatic
                case AutomaticRange automaticRange:
                    throw new NotImplementedException("Automatic range notation is not supported yet.");

                // fixed number
                case NumericRange numericRange:
                    repetitions = numericRange.Repetitions;
                    break;

                // value of some named token
                case TokenRange tokenRange:
                    object referencedValue = FindNamedTokenValue(tokenRange.ReferencedName, scope, symbolTable);
                    if (referencedValue is int value)
                        repetitions = value;
                    else
                        throw new ParserException($"Could not use the non-numeric value '{referencedValue}' " +
                                                  $"of token '{tokenRange.ReferencedName}' as an repetition number.");

                    break;
            }

            // parse the inner scope a number of times
            var expandedList = new List<object>();
            for (var i = 0; i < repetitions; i++)
            {
                Dictionary<string, object> innerRepeatedDict = ParseScope(node.InnerScope, symbolTable);
                expandedList.Add(innerRepeatedDict.First().Value);
            }

            // find a name
            string name = "listToken";
            if (node.InnerScope.Count == 1)
            {
                if (node.InnerScope.First().Token is NamedToken named) name = named.Name + "s";
            }

            dict.Add(name, expandedList);
        }

        private void ParseNamedToken(NamedToken namedToken, IDictionary<string, object> dict)
        {
            ParseNamedToken(namedToken, out object result);
            if (!namedToken.HasStructuredName)
            {
                dict.Add(namedToken.Name, result);
            }
            else
            {
                IDictionary<string, object> innerDict = dict;
                foreach (string name in namedToken.StructuredName.Take(namedToken.StructuredName.Length - 1))
                {
                    if (innerDict.TryGetValue(name, out object obj))
                    {
                        // append value to inner dictionary
                        if (obj is Dictionary<string, object> newInnerDict)
                            innerDict = newInnerDict;
                        else
                            throw new ParserException($"Structured named token '{namedToken.Name}' can not be set, because the " +
                                                      $"inner key '{name}' is already set to a non-object type.");
                    }
                    else
                    {
                        // create a new inner dictionary
                        var newInnerDict = new Dictionary<string, object>();
                        innerDict.Add(name, newInnerDict);
                        innerDict = newInnerDict;
                    }
                }

                innerDict.Add(namedToken.StructuredName.Last(), result);
            }
        }

        private void ParseNamedToken(NamedToken namedToken, out object result)
        {
            string input = NextInputToken();
            switch (namedToken.Type)
            {
                case TokenType.Auto:
                    if (int.TryParse(input, out int autoInt)) result = autoInt;
                    else if (long.TryParse(input, out long autoLong)) result = autoLong;
                    else if (TryParseFloat(input, out float autoFloat)) result = autoFloat;
                    else if (bool.TryParse(input, out bool autoBool)) result = autoBool;
                    else result = input;
                    break;

                case TokenType.Int:
                    if (int.TryParse(input, out int parsedInt)) result = parsedInt;
                    else if (long.TryParse(input, out long autoLong)) result = autoLong;
                    else throw new ParserException($"Could not parse '{input}' to expected type {TokenType.Int}.");
                    break;

                case TokenType.Bool:
                    if (bool.TryParse(input, out bool parsedBool)) result = parsedBool;
                    else throw new ParserException($"Could not parse '{input}' to expected type {TokenType.Bool}.");
                    break;

                case TokenType.Float:
                    if (TryParseFloat(input, out float parsedFloat)) result = parsedFloat;
                    else throw new ParserException($"Could not parse '{input}' to expected type {TokenType.Float}.");
                    break;

                case TokenType.String:
                    result = input;
                    break;

                default:
                    throw new ParserException($"Unexpected type '{namedToken.Type}' for named token.");
            }
        }

        private void ParseLiteralToken(LiteralToken literalToken)
        {
            // skip empty literal tokens
            if (literalToken.Content.Length > 0)
            {
                string input = NextInputToken();
                if (!input.StartsWith(literalToken.Content))
                    throw new ParserException($"Expected literal content '{literalToken.Content}' instead of '{input}'.");

                // partial match
                if (input.Length != literalToken.Content.Length)
                {
                    throw new NotImplementedException("Partial string matching is not supported yet.");
                }
            }
        }

        private void ParseRegExToken(RegExToken regExToken, out object result)
        {
            // skip empty patterns
            if (regExToken.Pattern.Length == 0)
            {
                result = "";
            }
            else
            {
                string input = NextInputToken();
                Match match = regExToken.IsCompiled ? regExToken.CompiledRegex.Match(input) : Regex.Match(input, regExToken.Pattern);

                if (match.Success) result = match.Value;
                else throw new ParserException($"Could not match regex '{regExToken.Pattern}' on '{input}'.");
            }
        }

        /* helper */

        private static object FindNamedTokenValue(string name, Scope scope,
            IReadOnlyDictionary<Scope, Dictionary<string, object>> symbolTable)
        {
            Scope enclosingScope = scope.FindNamedToken(name).Scope;
            if (symbolTable.TryGetValue(enclosingScope, out Dictionary<string, object> key))
            {
                return key[name];
            }

            throw new ParserException($"Referenced token '{name}' doesn't exist within the current scope.");
        }

        private bool TryParseFloat(string s, out float result)
        {
            return float.TryParse(s, NumberStyles.Float | NumberStyles.AllowLeadingSign, _culture, out result);
        }
    }
}
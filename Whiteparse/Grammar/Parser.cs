using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;
using Whiteparse.Grammar.Tokens;
using Whiteparse.Grammar.Tokens.Ranges;
using static Sprache.Parse;

namespace Whiteparse.Grammar
{
    /// <summary>
    /// Parses the Whiteparse grammar to a <see cref="Specification"/> object
    /// </summary>
    public static class Parser
    {
        /// <summary>Every token has to start with this character. Variables will start with two occurences of this character.</summary>
        private const char TOKEN_PREFIX = '$';

        /// <summary>If this character is immediately followed by the token prefix, the token is flagged as hidden</summary>
        private const char HIDDEN_PREFIX = '?';

        /// <summary>Content beyond this character is ignored until the end of the line (except if it occurs within strings or tokens)</summary>
        private const char COMMENT_PREFIX = '#';

        /// <summary>Character used for escaping special character</summary>
        private const char ESCAPE_CHARACTER = '\\';

        /// <summary>The character which separates the variable name from the variable content</summary>
        private const char VARIABLE_DEFINITION_SIGN = '=';

        /// <summary>Reserved token identifier for the newline token</summary>
        private const char NEWLINE_TOKEN_IDENTIFIER = '.';

        /// <summary>The character which separates field names in structured identifiers</summary>
        private const char STRUCTURED_IDENTIFIER_DELIMITER = '.';

        /* identifiers */

        private static readonly Parser<string> Identifier =
            LetterOrDigit.AtLeastOnce().Text()
                .Named("identifier");

        private static readonly Parser<IEnumerable<string>> StructuredIdentifier =
            Identifier.DelimitedBy(Char(STRUCTURED_IDENTIFIER_DELIMITER))
                .Named("dotted identifier");

        /* token definitions */

        private static readonly Parser<Token> NewLineTokenDefinition =
            Char(NEWLINE_TOKEN_IDENTIFIER).Return(new NewLineToken())
                .Named("new line token");

        private static readonly Func<bool, Parser<Token>> NamedTokenDefinition = hidden => (
                from type in TokenType.XOr(Return(Tokens.TokenType.Auto))
                from name in StructuredIdentifier
                select new NamedToken(name.ToArray(), type, hidden))
            .Named("token name");

        private static readonly Parser<Token> LiteralTokenDefinition = (
                from left in Char('"')
                from content in CharsExceptEscaped('"').Many().Text()
                from right in Char('"')
                select new LiteralToken(content))
            .XOr(
                from left in Char('\'')
                from content in CharsExceptEscaped('\'').Many().Text()
                from right in Char('\'')
                select new LiteralToken(content))
            .Named("literal token");

        private static readonly Func<bool, Parser<Token>> RegExTokenDefinition = hidden => (
                from left in Char('(')
                from content in RegExPattern
                from right in Char(')')
                select new RegExToken(content, hidden))
            .Named("regex token");

        private static readonly Parser<string> RegExPattern =
            AnyCharExceptEscaped('(', ')').XOr(
                    from left in Char('(')
                    from content in RegExPattern
                    from right in Char(')')
                    select '(' + content + ')')
                .Many().Select(string.Concat)
                .Named("regex pattern");

        private static readonly Parser<TokenType> TokenType = (
                from left in Char('[')
                from typeCode in String("int").Return(Tokens.TokenType.Int)
                    .XOr(String("float").Return(Tokens.TokenType.Float))
                    .XOr(String("string").Return(Tokens.TokenType.String))
                    .XOr(String("bool").Return(Tokens.TokenType.Bool))
                from right in Char(']')
                select typeCode)
            .Named("token type");

        /* list tokens */

        private static readonly Parser<(IRangeSpecifier range, string[] delimiters)> ListTokenExtension = (
                from left in Char('{')
                from range in Char('+').Return(AutomaticRange.FromType(AutomaticRangeType.AtLeastOnce))
                    .XOr(String("*?").Return(AutomaticRange.FromType(AutomaticRangeType.ManyLazy))
                        .Or(Char('*').Return(AutomaticRange.FromType(AutomaticRangeType.Many))))
                    .XOr<IRangeSpecifier>(Number.Then(n => Return(new NumericRange(int.Parse(n)))))
                    .XOr(Char(TOKEN_PREFIX).Then(d =>
                        Identifier.Then(name => Return(new TokenRange(name)))))
                from delimiters in
                    Char(':').Then(c => CharsExceptEscaped(':', '{', '}').AtLeastOnce().Text().DelimitedBy(Char(':')))
                        .Optional()
                from right in Char('}')
                select (range, delimiters.GetOrDefault()?.ToArray()))
            .Named("range and/or delimiter specification");

        private static readonly Parser<InlineListToken> InlineListToken = (
                from left in Char('[')
                from tokens in Token.XOr(Text).SingleLineSuperToken().Many()
                from right in Char(']')
                select new InlineListToken(tokens))
            .Named("inline list");

        /* tokens */

        private static readonly Parser<Token> TokenDefinition =
            Char(TOKEN_PREFIX).Then(e =>
                    NewLineTokenDefinition
                        .XOr(LiteralTokenDefinition
                            .XOr(Char(HIDDEN_PREFIX).Optional().Then(hidden =>
                                NamedTokenDefinition(hidden.IsDefined)
                                    .XOr(RegExTokenDefinition(hidden.IsDefined))))))
                .Named("token definition");

        private static readonly Parser<Token> Token = (
                from token in TokenDefinition.XOr(InlineListToken)
                from listExt in ListTokenExtension.Many()
                select listExt.Any() ? UnwrapListExtensions(token, listExt) : token)
            .Named("token");

        private static ListToken UnwrapListExtensions(Token inner, IEnumerable<(IRangeSpecifier, string[])> listExtensions)
        {
            return listExtensions.Aggregate(inner,
                (Token current, (IRangeSpecifier range, string[] delimiters) extension)
                    => new ListToken(current, extension.range, extension.delimiters)) as ListToken;
        }

        /* text */

        private static readonly Parser<char> TextCharacter =
            CharsExceptEscaped(TOKEN_PREFIX, COMMENT_PREFIX, '(', ')', '{', '}', '[', ']');

        private static readonly Parser<LiteralToken> Text = (
                from content in TextCharacter.Except(WhiteSpace).AtLeastOnce().Text()
                select new LiteralToken(content))
            .Named("text");

        /* variable statements */

        private static readonly Parser<Variable> Variable = (
                from dollars in Char(TOKEN_PREFIX).Repeat(2)
                from type in ObjectType.Optional()
                from name in Identifier
                from leading in WhiteSpace.Many()
                from sign in Char(VARIABLE_DEFINITION_SIGN)
                from trailing in WhiteSpace.Many()
                from tokens in Token.XOr(Text).SingleLineSuperToken().Many()
                select new Variable(name, tokens, type.GetOrDefault()))
            .Named("variable definition");

        private static readonly Parser<string> ObjectType = (
                from left in Char('[')
                from typeCode in LetterOrDigit.XOr(Char('_')).AtLeastOnce().Text()
                    .DelimitedBy(Char('.')).Select(e => string.Join(".", e))
                from right in Char(']')
                select typeCode)
            .Named("variable object type");

        /* helpers */

        private static readonly CommentParser Comment =
            new CommentParser(COMMENT_PREFIX.ToString(), null, null, "\n");

        private static readonly Parser<string> LineContinuation =
            from escape in Char(ESCAPE_CHARACTER)
            from whitespace in WhiteSpace.Except(LineEnd).Many()
            from newline in LineEnd
            select newline;

        /// <summary>
        /// Parse the token, embedded in any amount of whitespace characters or comments
        /// </summary>
        private static Parser<T> SuperToken<T>(this Parser<T> parser)
        {
            return from leading in WhiteSpace.Once().XOr(Comment.SingleLineComment).Many()
                from item in parser
                from trailing in WhiteSpace.Once().XOr(Comment.SingleLineComment).Many()
                select item;
        }

        /// <summary>
        /// Parse the token, embedded in any amount of whitespace (except line breaks) characters or comments.
        /// If the line es ended with a escape character, multiple lines are possible.
        /// </summary>
        private static Parser<T> SingleLineSuperToken<T>(this Parser<T> parser)
        {
            return from leading in WhiteSpace.Except(LineEnd).Once().XOr(LineContinuation).XOr(Comment.SingleLineComment).Many()
                from item in parser
                from trailing in WhiteSpace.Except(LineEnd).Once().XOr(LineContinuation).XOr(Comment.SingleLineComment).Many()
                select item;
        }

        /// <summary>
        /// Parses any character except those in the given parameters, unless they have been escaped.
        /// The escape character will not be returned in the output.
        /// If the escape character appears twice, the escape character is parsed only once (escaping the escape character).
        /// If a escape character attempts to escape a character not in the given parameters, the parser fails (unrecognized escape character).
        /// The parser also fails if line end characters are parsed.
        /// </summary>
        /// <param name="chars">The only characters which can (and must) be escaped</param>
        /// <returns>The parsed or escaped character</returns>
        private static Parser<char> CharsExceptEscaped(params char[] chars)
        {
            return Char(ESCAPE_CHARACTER).Then(e => Char(ESCAPE_CHARACTER).XOr(Chars(chars)))
                .XOr(CharExcept(chars).Except(LineEnd));
        }

        /// <summary>
        /// Parses any character except those in the given parameters, unless they have been escaped.
        /// The escape character will also be returned in the output.
        /// The parser fails if line end characters are parsed.
        /// </summary>
        /// <param name="chars">Characters which are only parsed if they are escaped</param>
        /// <returns>Any parsed character (except line ends) or two characters if a character was escaped</returns>
        private static Parser<string> AnyCharExceptEscaped(params char[] chars)
        {
            return Char(ESCAPE_CHARACTER)
                .Then(e => AnyChar.Except(LineEnd).Then(c => Return(string.Concat(ESCAPE_CHARACTER, c))))
                .XOr(CharExcept(chars).Except(LineEnd).Once().Text());
        }

        /* grammar specification */

        private static readonly Parser<Specification> Specification = (
                from elements in Token
                    .Or<object>(Variable)
                    .XOr(Text)
                    .SuperToken().Many().Optional()
                select new Specification(elements.GetOrDefault().OfType<Token>().ToList(),
                    elements.GetOrDefault().OfType<Variable>()))
            .SuperToken().End()
            .Named("grammar specification");

        /// <summary>
        /// Parse the grammar specification from text input to a <see cref="Specification"/> object
        /// </summary>
        /// <exception cref="GrammarException">An error occured during grammar parsing or semantic resolution</exception>
        public static Specification Parse(string input)
        {
            try
            {
                return Specification.Parse(input);
            }
            catch (ParseException e)
            {
                throw new GrammarException("An error occurred while parsing the grammar specification. " + e.Message, e);
            }
        }
    }
}
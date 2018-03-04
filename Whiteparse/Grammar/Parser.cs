using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;
using Whiteparse.Grammar.Tokens;
using Whiteparse.Grammar.Tokens.Ranges;
using static Sprache.Parse;

namespace Whiteparse.Grammar
{
    public static class Parser
    {
        /** Each token has to start with this symbol. Variables will start with two occurences of this symbol. */
        private const char TOKEN_PREFIX = '$';

        /** If this symbol is immediately followed by the token prefix, the token is flagged as hidden */
        private const char HIDDEN_PREFIX = '?';

        /** Content beyond this symbol is ignored until the end of the line (except if it occurs within strings or tokens) */
        private const char COMMENT_PREFIX = '#';

        /** The symbol which separates the variable name from the variable content */
        private const char VARIABLE_DEFINITION_SIGN = '=';

        /** Character used for escaping symbols */
        private const char ESCAPE_CHARACTER = '\\';

        /** Reserved token identifier for the newline token */
        private const char NEWLINE_TOKEN_IDENTIFIER = ';';

        /* comments */

        private static readonly CommentParser Comment = new CommentParser(COMMENT_PREFIX.ToString(), null, null, "\n");

        /* escaped new line wrapping */

        private static readonly Parser<string> EscapedNewLine =
            from escape in Char(ESCAPE_CHARACTER)
            from whitespace in WhiteSpace.Except(LineEnd).Many()
            from newline in LineEnd
            select newline;

        /* parenthesized strings */

        private static readonly Parser<string> ParenthesizedRegExString =
            AnyCharExceptEscaped('(', ')')
                .XOr(
                    from left in Char('(')
                    from content in ParenthesizedRegExString
                    from right in Char(')')
                    select '(' + content + ')')
                .Many().Select(string.Concat)
                .Named("string with parenthesis");

        /* identifiers */

        private static readonly Parser<string> IdentifierParser =
            LetterOrDigit.AtLeastOnce().Text()
                .Named("identifier");

        private static readonly Parser<IEnumerable<string>> DottedIdentifierParser =
            LetterOrDigit.AtLeastOnce().Text().DelimitedBy(Char('.'))
                .Or(LetterOrDigit.AtLeastOnce().Text().Select(e => new[] {e}))
                .Named("dotted identifier");

        /* inner token definitions */

        private static readonly Parser<TokenDataType> TokenTypeParser = (
                from left in Char('[')
                from typeCode in String("int").Return(TokenDataType.Int)
                    .XOr(String("float").Return(TokenDataType.Float))
                    .XOr(String("string").Return(TokenDataType.String))
                    .XOr(String("bool").Return(TokenDataType.Bool))
                from right in Char(']')
                select typeCode)
            .Named("token type code");

        private static readonly Parser<Token> NewLineTokenParser =
            Char(NEWLINE_TOKEN_IDENTIFIER).Return(new NewLineToken())
                .Named("new line token");

        private static readonly Func<bool, Parser<Token>> NamedTokenParser =
            hidden => (
                    from type in TokenTypeParser.Or(Return(TokenDataType.Auto))
                    from name in DottedIdentifierParser
                    select new NamedToken(name.ToArray(), type, hidden))
                .Named("token name");

        private static readonly Parser<Token> LiteralTokenParser = (
                from left in Char('"')
                from content in CharsExceptEscaped('"').Many().Text()
                from right in Char('"')
                select new LiteralToken(content))
            .Named("literal token");

        private static readonly Func<bool, Parser<Token>> RegExTokenParser =
            hidden => (
                    from left in Char('(')
                    from content in ParenthesizedRegExString
                    from right in Char(')')
                    select new RegExToken(content, hidden))
                .Named("regex token");

        /* list token extension */

        private static readonly Parser<(IRangeSpecifier, string[])> ListTokenExtensionParser = (
                from left in Char('{')
                from range in Char('+').Return(AutomaticRange.FromType(AutomaticRangeType.AtLeastOnce))
                    .XOr(String("*?").Return(AutomaticRange.FromType(AutomaticRangeType.ManyLazy))
                        .Or(Char('*').Return(AutomaticRange.FromType(AutomaticRangeType.Many))))
                    .XOr<IRangeSpecifier>(Number.Then(n => Return(new NumericRange(int.Parse(n)))))
                    .XOr(Char(TOKEN_PREFIX)
                        .Then(d => IdentifierParser
                            .Then(name => Return(new TokenRange(new NamedToken(name))))))
                from delimiters in Char(':')
                    .Then(c => CharsExceptEscaped(':', '}').AtLeastOnce().Text().DelimitedBy(Char(':')))
                    .Optional()
                from right in Char('}')
                select (range, delimiters.IsDefined ? delimiters.Get().ToArray() : null))
            .Named("range and/or delimiter specification");

        /* tokens */

        private static readonly Parser<Token> TokenDeclarationParser = (
                from token in NewLineTokenParser
                    .XOr(LiteralTokenParser
                        .XOr(Char(HIDDEN_PREFIX).Optional()
                            .Then(hidden =>
                                NamedTokenParser(hidden.IsDefined)
                                    .XOr(RegExTokenParser(hidden.IsDefined)))))
                from listExt in ListTokenExtensionParser.Many()
                select listExt.Any() ? ListTokenExtensionsWrapper(token, listExt) : token)
            .Named("token declaration");

        private static readonly Parser<Token> TokenParser = (
                from dollar in Char(TOKEN_PREFIX)
                from token in TokenDeclarationParser
                select token)
            .Named("token");

        /* inline list token */

        private static readonly Parser<InlineListToken> InlineListTokenParser = (
                from left in Char('[')
                from tokens in TokenParser.XOr(TextParser).SingleLineSuperToken().Many()
                from right in Char(']')
                select new InlineListToken(tokens))
            .Named("inline list");

        /* text (parsed as literal tokens) */

        private static readonly Parser<char> NoWhiteSpaceOrPrefixOrComment =
            CharsExceptEscaped(TOKEN_PREFIX, COMMENT_PREFIX, '{', '}', '[', ']').Except(WhiteSpace);

        private static readonly Parser<LiteralToken> TextParser = (
                from content in NoWhiteSpaceOrPrefixOrComment.AtLeastOnce().Text().Or(Comment.SingleLineComment)
                select new LiteralToken(content))
            .Named("text");

        /* variable statements */

        private static readonly Parser<string> VariableObjectTypeParse = (
                from left in Char('[')
                from typeCode in LetterOrDigit.Or(Char('_')).AtLeastOnce().Text()
                    .DelimitedBy(Char('.')).Select(e => string.Join(".", e))
                    .Or(LetterOrDigit.Or(Char('_')).AtLeastOnce().Text())
                from right in Char(']')
                select typeCode)
            .Named("variable object type");

        private static readonly Parser<Variable> VariableParser = (
                from firstDollar in Char(TOKEN_PREFIX)
                from secondDollar in Char(TOKEN_PREFIX)
                from type in VariableObjectTypeParse.Optional()
                from name in IdentifierParser
                from leadingWhitespace in WhiteSpace.Many()
                from equal in Char(VARIABLE_DEFINITION_SIGN)
                from trailingWhitespace in WhiteSpace.Many()
                from tokens in TokenParser.XOr(TextParser).SingleLineSuperToken().Many()
                select new Variable(name, tokens, type.GetOrDefault()))
            .Named("variable definition");

        /* grammar specification */

        private static readonly Parser<Specification> SpecificationParser = (
                from elements in TokenParser
                    .Or<object>(VariableParser)
                    .XOr(InlineListTokenParser)
                    .XOr(TextParser)
                    .SuperToken().Many().Optional()
                select new Specification(
                    elements.IsDefined ? elements.Get().OfType<Token>() : null,
                    elements.IsDefined ? elements.Get().OfType<Variable>() : null))
            .SuperToken()
            .End()
            .Named("grammar specification");

        /**
         * Parse the token, embedded in any amount of whitespace characters or comments
         */
        private static Parser<T> SuperToken<T>(this Parser<T> parser)
        {
            // TODO: are leading comments even possible?
            return from leading in WhiteSpace.Once()
                    .XOr(Comment.SingleLineComment)
                    .Many()
                from item in parser
                from trailing in WhiteSpace.Once()
                    .XOr(Comment.SingleLineComment)
                    .Many()
                select item;
        }

        /**
         * Parse the token, embedded in any amount of whitespace (except line breaks) characters or comments.
         * If the line es ended with a escape character, multiple lines are possible.
         */
        private static Parser<T> SingleLineSuperToken<T>(this Parser<T> parser)
        {
            // TODO: are leading new line escapes even possible?
            return from leading in WhiteSpace.Except(LineEnd).Once()
                    .XOr(EscapedNewLine)
                    .XOr(Comment.SingleLineComment)
                    .Many()
                from item in parser
                from trailing in WhiteSpace.Except(LineEnd).Once()
                    .XOr(EscapedNewLine)
                    .XOr(Comment.SingleLineComment)
                    .Many()
                select item;
        }

        /// <summary>
        /// Parses any character except those in the given parameters, unless they have been escaped.
        /// The escape character will not be returned.
        /// If the escape character appears twice, the escape character is parsed only once (escaping the escape character).
        /// If a escape character attempts to escape a character not in the given parameters, the parser fails.
        /// The parser also fails if line end characters are parsed.
        /// </summary>
        /// <param name="chars">Characters which need to be escaped</param>
        /// <returns>Any parsed or escaped character (except line ends)</returns>
        private static Parser<char> CharsExceptEscaped(params char[] chars)
        {
            return Char(ESCAPE_CHARACTER).Then(e => Char(ESCAPE_CHARACTER).XOr(Chars(chars)))
                .XOr(CharExcept(chars).Except(LineEnd))
                .Named("any (escaped) character except line ends, " + string.Join(", ", chars));
        }

        /// <summary>
        /// Parses any character except those in the given parameters, unless they have been escaped.
        /// Escape characters will always be parsed.
        /// The parser fails if line end characters are parsed.
        /// </summary>
        /// <param name="chars">Characters which are only parsed if they have been escaped</param>
        /// <returns>Any parsed character (except line ends) or two characters if a character was escaped</returns>
        private static Parser<string> AnyCharExceptEscaped(params char[] chars)
        {
            return Char(ESCAPE_CHARACTER).Then(e => AnyChar.Except(LineEnd).Then(c => Return(string.Concat(ESCAPE_CHARACTER, c))))
                .XOr(CharExcept(chars).Except(LineEnd).Once().Text());
        }

        private static ListToken ListTokenExtensionsWrapper(Token inner, IEnumerable<(IRangeSpecifier, string[])> listExtensions)
        {
            return listExtensions.Aggregate(inner,
                (Token current, (IRangeSpecifier range, string[] delimiters) extension)
                    => new ListToken(current, extension.range, extension.delimiters)) as ListToken;
        }

        /**
         * Parse the grammar specification from text input
         */
        public static Specification Parse(string input)
        {
            return SpecificationParser.Parse(input);
        }
    }
}
using DeepEqual.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Whiteparse.Grammar;
using Xunit;
using Xunit.Abstractions;

namespace Whiteparse.Test
{
    public abstract class AbstractTestSuite
    {
        private readonly ITestOutputHelper output;

        protected AbstractTestSuite(ITestOutputHelper output)
        {
            this.output = output;
        }

        /* Utils for the Grammar */

        protected static void ParseGrammer(string grammar)
        {
            Specification.FromString(grammar);
        }

        protected void CompareGrammar(string grammar, Specification expectedSpecification)
        {
            Specification parsed = Specification.FromString(grammar);

            output.WriteLine(parsed.ToString());
            parsed.WithDeepEqual(expectedSpecification).IgnoreSourceProperty(e => e.GlobalScope).Assert();
        }

        protected void FailGrammar(string grammar)
        {
            Assert.ThrowsAny<GrammarException>(() =>
            {
                try
                {
                    Specification parsed = Specification.FromString(grammar);

                    // print additional details if the parsing indeed worked
                    output.WriteLine("Parsing should have thrown an exception, but this object was parsed successfully:");
                    output.WriteLine("");
                    output.WriteLine(parsed.ToString());
                }
                catch (GrammarException e)
                {
                    output.WriteLine(e.Message);
                    throw;
                }
            });
        }

        /* Utils for the Parser */

        protected static void ParseInput(string grammar, string input)
        {
            Parser.FromSpecificationString(grammar).ParseObject(input);
        }

        protected void CompareResult(string grammar, string input, object expectedObject)
        {
            object parsed = Parser.FromSpecificationString(grammar).ParseObject(input);

            output.WriteLine(JsonConvert.SerializeObject(parsed, Formatting.Indented));
            parsed.ShouldDeepEqual(expectedObject);
        }

        protected void CompareResultJson(string grammar, string input, object expectedObject)
        {
            string parsed = Parser.FromSpecificationString(grammar).ParseJson(input);
            object converted = JsonConvert.DeserializeObject(parsed) as JObject;

            output.WriteLine(JsonConvert.SerializeObject(parsed, Formatting.Indented));
            converted.ShouldDeepEqual(expectedObject);
        }
    }
}
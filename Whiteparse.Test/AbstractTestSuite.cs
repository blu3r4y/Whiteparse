using System.Globalization;
using DeepEqual.Syntax;
using Newtonsoft.Json;
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
            Template.FromString(grammar);
        }

        protected void CompareGrammar(string grammar, Template expectedTemplate)
        {
            Template parsed = Template.FromString(grammar);

            output.WriteLine(parsed.ToString());
            parsed.ShouldDeepEqual(expectedTemplate);
        }

        protected void FailGrammar(string grammar)
        {
            Assert.ThrowsAny<GrammarException>(() =>
            {
                try
                {
                    Template parsed = Template.FromString(grammar);

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

        /* Utils for the TemplateParser */

        protected void ParseInput(string grammar, string input)
        {
            object parsed = Whiteparser.ParseObject(grammar, input, CultureInfo.InvariantCulture);
            
            output.WriteLine(JsonConvert.SerializeObject(parsed, Formatting.Indented));
        }

        protected void CompareResult(string grammar, string input, object expectedObject)
        {
            object parsed = Whiteparser.ParseObject(grammar, input, CultureInfo.InvariantCulture);

            output.WriteLine(JsonConvert.SerializeObject(parsed, Formatting.Indented));
            parsed.ShouldDeepEqual(expectedObject);
        }

        protected void CompareResultJson(string grammar, string input, object expectedObject)
        {
            string parsed = Whiteparser.ParseJson(grammar, input, CultureInfo.InvariantCulture);
            var converted = JsonConvert.DeserializeObject(parsed);

            output.WriteLine(JsonConvert.SerializeObject(converted, Formatting.Indented));
            converted.ShouldDeepEqual(expectedObject);
        }
    }
}
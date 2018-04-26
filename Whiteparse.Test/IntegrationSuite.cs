using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Whiteparse.Test
{
    public class IntegrationSuite : AbstractTestSuite
    {
        public IntegrationSuite(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("blockchain")]
        [InlineData("bowling")]
        [InlineData("datacenter")]
        [InlineData("delivery")]
        [InlineData("drones")]
        [InlineData("genetic")]
        [InlineData("hyperloop")]
        [InlineData("loon")]
        [InlineData("pong")]
        [InlineData("router")]
        [InlineData("satellites")]
        [InlineData("selfdriving")]
        [InlineData("streaming")]
        [InlineData("streetview")]
        [InlineData("surgery")]
        public void ParseGrammar(string filename)
        {
            string grammar = File.ReadAllText($".\\TestData\\integration\\{filename}.whp");
            ParseGrammer(grammar);
        }

        [Theory(Skip = "Deep equality is hard between JObjects and ExpandoObjects")]
        [InlineData("blockchain")]
        [InlineData("bowling")]
        [InlineData("datacenter")]
        [InlineData("delivery")]
        [InlineData("drones")]
        [InlineData("genetic")]
        [InlineData("hyperloop")]
        [InlineData("loon")]
        [InlineData("pong")]
        [InlineData("router")]
        [InlineData("satellites")]
        [InlineData("selfdriving")]
        [InlineData("streaming")]
        [InlineData("streetview")]
        [InlineData("surgery")]
        public void ParseObject(string filename)
        {
            string input = ReadAllTextSkipComments($".\\TestData\\integration\\{filename}.in");
            string grammar = File.ReadAllText($".\\TestData\\integration\\{filename}.whp");
            object expected = JsonConvert.DeserializeObject(File.ReadAllText($".\\TestData\\integration\\{filename}.json")) as JObject;

            CompareResult(grammar, input, expected);
        }

        [Theory]
        [InlineData("blockchain")]
        [InlineData("bowling")]
        [InlineData("datacenter")]
        [InlineData("delivery")]
        [InlineData("drones")]
        [InlineData("genetic")]
        [InlineData("hyperloop")]
        [InlineData("loon")]
        [InlineData("pong")]
        [InlineData("router")]
        [InlineData("satellites")]
        [InlineData("selfdriving")]
        [InlineData("streaming")]
        [InlineData("streetview")]
        [InlineData("surgery")]
        public void ParseJson(string filename)
        {
            string input = ReadAllTextSkipComments($".\\TestData\\integration\\{filename}.in");
            string grammar = File.ReadAllText($".\\TestData\\integration\\{filename}.whp");
            object expected = JsonConvert.DeserializeObject(File.ReadAllText($".\\TestData\\integration\\{filename}.json")) as JObject;

            CompareResultJson(grammar, input, expected);
        }

        [Theory]
        [InlineData("blockchain")]
        [InlineData("datacenter")]
        [InlineData("delivery")]
        [InlineData("hyperloop")]
        [InlineData("loon")]
        [InlineData("router")]
        [InlineData("satellites")]
        [InlineData("selfdriving")]
        [InlineData("streaming")]
        [InlineData("streetview")]
        public void ParseHugeInput(string filename)
        {
            string input = ReadAllTextSkipComments($".\\TestData\\integration\\huge\\{filename}.in");
            string grammar = File.ReadAllText($".\\TestData\\integration\\{filename}.whp");

            ParseInput(grammar, input);
        }

        private static string ReadAllTextSkipComments(string filename)
        {
            return string.Join('\n', File.ReadAllLines(filename).Where(line => !line.StartsWith("#")));
        }
    }
}
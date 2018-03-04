using Newtonsoft.Json;
using Whiteparse.Grammar;

namespace Whiteparse
{
    public class Parser
    {
        public Specification Specification { get; }

        public Parser(Specification specification)
        {
            Specification = specification;
        }

        public object ParseObject(string input)
        {
            return null;
        }

        public string ParseJson(string input)
        {
            return JsonConvert.SerializeObject(ParseObject(input));
        }

        public static Parser FromSpecificationString(string input)
        {
            return new Parser(Specification.FromString(input));
        }
    }
}
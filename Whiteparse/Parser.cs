using Newtonsoft.Json;
using Whiteparse.Grammar;

namespace Whiteparse
{
    public class Parser
    {
        public Template Template { get; }

        public Parser(Template template)
        {
            Template = template;
        }

        public object ParseObject(string input)
        {
            return null;
        }

        public string ParseJson(string input)
        {
            return JsonConvert.SerializeObject(ParseObject(input));
        }

        public static Parser FromTemplate(Template template)
        {
            return new Parser(template);
        }

        public static Parser FromTemplate(string template)
        {
            return FromTemplate(Template.FromString(template));
        }

        public static object ParseObject(Template template, string input)
        {
            return FromTemplate(template).ParseObject(input);
        }

        public static object ParseObject(string template, string input)
        {
            return FromTemplate(template).ParseObject(input);
        }

        public static string ParseJson(Template template, string input)
        {
            return FromTemplate(template).ParseJson(input);
        }

        public static string ParseJson(string template, string input)
        {
            return FromTemplate(template).ParseJson(input);
        }
    }
}
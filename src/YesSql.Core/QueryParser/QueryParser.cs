using Parlot;
using Parlot.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using static Parlot.Fluent.Parsers;

namespace YesSql.Core.QueryParser
{
    public interface IQueryParser<T> where T : class
    {
        TermList<T> Parse(string text);
    }

    public class QueryParser<T> : IQueryParser<T> where T : class
    {
        private static Parser<List<V>> _customSeparated<U, V>(Parser<U> separator, Parser<V> parser) => new CustomSeparated<U, V>(separator, parser);

        public QueryParser(params TermParserBuilder<T>[] parsers)
        {
            var builtParsers = new List<Parser<TermNode>>();

            foreach(var p in parsers)
            {
                var termOption = new TermOption<T>(p.OneOrMany, p.TermQueryOption);
                TermOptions[p.Name] = termOption;
                builtParsers.Add(p.Parser);
            }

            var Terms = OneOf(builtParsers.ToArray());

            var Seperator = OneOf(parsers.Select(x => x.SeperatorParser).ToArray());

            // TODO this should be able to move to ZeroOrMany now.

            Parser = _customSeparated(
                Seperator,
                Terms)
                    .Then(static (context, terms) => 
                    {
                        var ctx = (QueryParseContext<T>)context;

                        return new TermList<T>(terms, ctx.TermOptions);
                    });
        }

        public Dictionary<string, TermOption<T>> TermOptions { get; } = new();

        protected Parser<TermList<T>> Parser { get; }

        public TermList<T> Parse(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return new TermList<T>();
            }

            var context = new QueryParseContext<T>(TermOptions, new Scanner(text));

            ParseResult<TermList<T>> result = default(ParseResult<TermList<T>>);
            if (Parser.Parse(context, ref result))
            {
                return result.Value;
            }
            else
            {
                return new TermList<T>();
            }
        }
    }

    public class QueryParseContext<T> : ParseContext where T : class
    {
        public QueryParseContext(Dictionary<string, TermOption<T>> termOptions, Scanner scanner, bool useNewLines = false) : base(scanner, useNewLines)
        {
            TermOptions = termOptions;
        }

        public Dictionary<string, TermOption<T>> TermOptions { get; }

        public TermOption<T> CurrentTermOption { get; set; }
    }
}

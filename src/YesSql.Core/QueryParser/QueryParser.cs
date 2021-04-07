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
        public QueryParser(params TermParserBuilder<T>[] parsers)
        {
            TermOptions = parsers.ToDictionary(k => k.Name, v => v.TermOption);

            var Terms = OneOf(parsers.Select(x => x.Parser).ToArray());

            Parser = ZeroOrMany(Terms)
                    .Then(static (context, terms) => 
                    {
                        var ctx = (QueryParseContext<T>)context;

                        return new TermList<T>(terms, ctx.TermOptions);
                    });                    
        }

        public IReadOnlyDictionary<string, TermOption<T>> TermOptions { get; }

        protected Parser<TermList<T>> Parser { get; }

        public TermList<T> Parse(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return new TermList<T>(TermOptions);
            }

            var context = new QueryParseContext<T>(TermOptions, new Scanner(text));

            ParseResult<TermList<T>> result = default(ParseResult<TermList<T>>);
            if (Parser.Parse(context, ref result))
            {
                return result.Value;
            }
            else
            {
                return new TermList<T>(TermOptions);
            }
        }
    }

    public class QueryParseContext<T> : ParseContext where T : class
    {
        public QueryParseContext(IReadOnlyDictionary<string, TermOption<T>> termOptions, Scanner scanner, bool useNewLines = false) : base(scanner, useNewLines)
        {
            TermOptions = termOptions;
        }

        public IReadOnlyDictionary<string, TermOption<T>> TermOptions { get; }

        public TermOption<T> CurrentTermOption { get; set; }
    }
}

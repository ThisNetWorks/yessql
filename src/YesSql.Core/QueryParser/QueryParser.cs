using System;
using System.Collections.Generic;
using Parlot;
using Parlot.Fluent;
using static Parlot.Fluent.Parsers;

namespace YesSql.Core.QueryParser
{
    public interface IQueryParser<T> where T : class
    {
        TermList<T> Parse(string text);
    }

    public class QueryParser<T> : IQueryParser<T> where T : class
    {
        private Dictionary<string, TermOption<T>> _termOptions;
        private Parser<TermList<T>> _parser;

        public QueryParser(Parser<TermNode>[] termParsers, Dictionary<string, TermOption<T>> termOptions)
        {
            _termOptions = termOptions;

            var terms = OneOf(termParsers);

            _parser = ZeroOrMany(terms)
                    .Then(static (context, terms) => 
                    {
                        var ctx = (QueryParseContext<T>)context;

                        return new TermList<T>(terms, ctx.TermOptions);
                    });                    
        }

        public TermList<T> Parse(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return new TermList<T>(_termOptions);
            }

            var context = new QueryParseContext<T>(_termOptions, new Scanner(text));

            ParseResult<TermList<T>> result = default(ParseResult<TermList<T>>);
            if (_parser.Parse(context, ref result))
            {
                return result.Value;
            }
            else
            {
                return new TermList<T>(_termOptions);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace YesSql.Core.QueryParser.Builders
{
    public class QueryParserBuilder<T> where T : class
    {
        private Dictionary<string, TermParserBuilder<T>> _termBuilders = new Dictionary<string, TermParserBuilder<T>>();

        public QueryParserBuilder<T> SetTermParser(TermParserBuilder<T> builder)
        {
            _termBuilders[builder.Name] = builder;

            return this;
        }

        public QueryParser<T> Build()
        {
            var builders = _termBuilders.Values.Select(x => x.Build());

            var parsers = builders.Select(x => x.Parser).ToArray();
            var termOptions = builders.Select(x => x.TermOption).ToDictionary(k => k.Name, v => v);

            return new QueryParser<T>(parsers, termOptions);
        }
    }

    public static class QueryParserBuilderExtensions
    {
        public static QueryParserBuilder<T> WithNamedTerm<T>(this QueryParserBuilder<T> builder, string name, Action<NamedTermParserBuilder<T>> action) where T : class
        {
            var parserBuilder = new NamedTermParserBuilder<T>(name);
            action(parserBuilder);

            builder.SetTermParser(parserBuilder);
            return builder;
        }

        public static QueryParserBuilder<T> WithDefaultTerm<T>(this QueryParserBuilder<T> builder, string name, Action<DefaultTermParserBuilder<T>> action) where T : class
        {
            var parserBuilder = new DefaultTermParserBuilder<T>(name);
            action(parserBuilder);

            builder.SetTermParser(parserBuilder);
            return builder;
        }        
    }
}

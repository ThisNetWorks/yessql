using System;
using System.Collections.Generic;
using System.Linq;

namespace YesSql.Core.FilterEngines.Builders
{
    public class QueryEngineBuilder<T> where T : class
    {
        private Dictionary<string, TermEngineBuilder<T, QueryTermOption<T>>> _termBuilders = new Dictionary<string, TermEngineBuilder<T, QueryTermOption<T>>>();

        public QueryEngineBuilder<T> SetTermParser(TermEngineBuilder<T, QueryTermOption<T>> builder)
        {
            _termBuilders[builder.Name] = builder;

            return this;
        }

        public QueryEngine<T> Build()
        {
            var builders = _termBuilders.Values.Select(x => x.Build());

            var parsers = builders.Select(x => x.Parser).ToArray();
            var termOptions = builders.Select(x => x.TermOption).ToDictionary(k => k.Name, v => v);

            return new QueryEngine<T>(parsers, termOptions);
        }
    }

    public static class QueryEngineBuilderExtensions
    {
        public static QueryEngineBuilder<T> WithNamedTerm<T>(this QueryEngineBuilder<T> builder, string name, Action<NamedTermEngineBuilder<T, QueryTermOption<T>>> action) where T : class
        {
            var parserBuilder = new NamedTermEngineBuilder<T, QueryTermOption<T>>(name);
            action(parserBuilder);

            builder.SetTermParser(parserBuilder);
            return builder;
        }

        public static QueryEngineBuilder<T> WithDefaultTerm<T>(this QueryEngineBuilder<T> builder, string name, Action<DefaultTermEngineBuilder<T, QueryTermOption<T>>> action) where T : class
        {
            var parserBuilder = new DefaultTermEngineBuilder<T, QueryTermOption<T>>(name);
            action(parserBuilder);

            builder.SetTermParser(parserBuilder);
            return builder;
        }
    }

    public class EnumerableEngineBuilder<T>
    {
        private Dictionary<string, TermEngineBuilder<T, EnumerableTermOption<T>>> _termBuilders = new Dictionary<string, TermEngineBuilder<T, EnumerableTermOption<T>>>();

        public EnumerableEngineBuilder<T> SetTermParser(TermEngineBuilder<T, EnumerableTermOption<T>> builder)
        {
            _termBuilders[builder.Name] = builder;

            return this;
        }

        public EnumerableEngine<T> Build()
        {
            var builders = _termBuilders.Values.Select(x => x.Build());

            var parsers = builders.Select(x => x.Parser).ToArray();
            var termOptions = builders.Select(x => x.TermOption).ToDictionary(k => k.Name, v => v);

            return new EnumerableEngine<T>(parsers, termOptions);
        }
    }

    public static class EnumerableEngineBuilderExtensions
    {
        public static EnumerableEngineBuilder<T> WithNamedTerm<T>(this EnumerableEngineBuilder<T> builder, string name, Action<NamedTermEngineBuilder<T, EnumerableTermOption<T>>> action) 
        {
            var parserBuilder = new NamedTermEngineBuilder<T, EnumerableTermOption<T>>(name);
            action(parserBuilder);

            builder.SetTermParser(parserBuilder);
            return builder;
        }

        public static EnumerableEngineBuilder<T> WithDefaultTerm<T>(this EnumerableEngineBuilder<T> builder, string name, Action<DefaultTermEngineBuilder<T, EnumerableTermOption<T>>> action) where T : class
        {
            var parserBuilder = new DefaultTermEngineBuilder<T, EnumerableTermOption<T>>(name);
            action(parserBuilder);

            builder.SetTermParser(parserBuilder);
            return builder;
        }
    }
}

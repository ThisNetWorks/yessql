using System;
using System.Collections.Generic;
using System.Linq;
using OrchardCore.Filters.Abstractions.Builders;
using OrchardCore.Filters.Enumerable.Services;

namespace OrchardCore.Filters.Enumerable
{
    public class EnumerableEngineBuilder<T>
    {
        private Dictionary<string, TermEngineBuilder<T, EnumerableTermOption<T>>> _termBuilders = new Dictionary<string, TermEngineBuilder<T, EnumerableTermOption<T>>>();

        public EnumerableEngineBuilder<T> SetTermParser(TermEngineBuilder<T, EnumerableTermOption<T>> builder)
        {
            _termBuilders[builder.Name] = builder;

            return this;
        }

        public IEnumerableParser<T> Build()
        {
            var builders = _termBuilders.Values.Select(x => x.Build());

            var parsers = builders.Select(x => x.Parser).ToArray();
            var termOptions = builders.Select(x => x.TermOption).ToDictionary(k => k.Name, v => v);

            return new EnumerableParser<T>(parsers, termOptions);
        }
    }
}

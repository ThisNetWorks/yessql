using System;
using OrchardCore.Filters.Builders;
using OrchardCore.Filters.Enumerable.Services;

namespace OrchardCore.Filters.Enumerable
{
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

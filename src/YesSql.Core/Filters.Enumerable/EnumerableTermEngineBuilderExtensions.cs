using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrchardCore.Filters.Abstractions.Builders;
using OrchardCore.Filters.Enumerable.Services;

namespace OrchardCore.Filters.Enumerable
{
    public static class EnumerableTermFilterBuilderExtensions
    {
        public static EnumerableUnaryEngineBuilder<T> OneCondition<T>(this TermEngineBuilder<T, EnumerableTermOption<T>> builder, Func<string, IEnumerable<T>, IEnumerable<T>> matchQuery)
        {
            Func<string, IEnumerable<T>, EnumerableExecutionContext<T>, ValueTask<IEnumerable<T>>> valueQuery = (q, val, ctx) => new ValueTask<IEnumerable<T>>(matchQuery(q, val));

            return builder.OneCondition(valueQuery);
        }

        public static EnumerableUnaryEngineBuilder<T> OneCondition<T>(this TermEngineBuilder<T, EnumerableTermOption<T>> builder, Func<string, IEnumerable<T>, EnumerableExecutionContext<T>, ValueTask<IEnumerable<T>>> matchQuery)
        {
            var operatorBuilder = new EnumerableUnaryEngineBuilder<T>(builder.Name, matchQuery);
            builder.SetOperator(operatorBuilder);

            return operatorBuilder;
        }

        public static EnumerableBooleanEngineBuilder<T> ManyCondition<T>(
            this TermEngineBuilder<T, EnumerableTermOption<T>> builder,
            Func<string, IEnumerable<T>, IEnumerable<T>> matchQuery,
            Func<string, IEnumerable<T>, IEnumerable<T>> notMatchQuery) 
        {
            Func<string, IEnumerable<T>, EnumerableExecutionContext<T>, ValueTask<IEnumerable<T>>> valueMatch = (q, val, ctx) => new ValueTask<IEnumerable<T>>(matchQuery(q, val));
            Func<string, IEnumerable<T>, EnumerableExecutionContext<T>, ValueTask<IEnumerable<T>>> valueNotMatch = (q, val, ctx) => new ValueTask<IEnumerable<T>>(notMatchQuery(q, val));

            return builder.ManyCondition(valueMatch, valueNotMatch);
        }

        public static EnumerableBooleanEngineBuilder<T> ManyCondition<T>(
            this TermEngineBuilder<T, EnumerableTermOption<T>> builder,
            Func<string, IEnumerable<T>, EnumerableExecutionContext<T>, ValueTask<IEnumerable<T>>> matchQuery,
            Func<string, IEnumerable<T>, EnumerableExecutionContext<T>, ValueTask<IEnumerable<T>>> notMatchQuery) 
        {
            var operatorBuilder = new EnumerableBooleanEngineBuilder<T>(builder.Name, matchQuery, notMatchQuery);
            builder.SetOperator(operatorBuilder);

            return operatorBuilder;
        }
    }
}
